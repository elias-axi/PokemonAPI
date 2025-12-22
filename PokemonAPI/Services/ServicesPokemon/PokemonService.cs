using Microsoft.EntityFrameworkCore;
using PokemonAPI.Data;
using PokemonAPI.Models.DTOs.Pokemon;
using PokemonAPI.Models.Entities;
using System.Text.Json;

namespace PokemonAPI.Services.ServicesPokemon
{
    public class PokemonService : IPokemonService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PokemonService> _logger;
        private const string PokeApiBaseUrl = "https://pokeapi.co/api/v2/";

        public PokemonService(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            ILogger<PokemonService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _logger = logger;
        }

        public async Task<PokemonResponseDto?> GetPokemonByNameAsync(string name)
        {
            try
            {
                var nombrePokemon = NormalizarNombrePokemon(name);

                var existePokemon = await _context.Pokemons
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == nombrePokemon);

                if (existePokemon != null)
                {
                    _logger.LogInformation("Pokémon {Name} encontrado en la base de datos", name);
                    return MappearDToPokemon(existePokemon);
                }

                var pokemonDeApi = await PokemonDeApiPublicaAsync(nombrePokemon);

                if (pokemonDeApi == null)
                {
                    _logger.LogWarning("Pokémon {Name} no encontrado en la PokeAPI", name);
                    return null;
                }

                _context.Pokemons.Add(pokemonDeApi);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pokémon {Name} guardado exitosamente en la base de datos", name);

                return MappearDToPokemon(pokemonDeApi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el Pokémon {Name}", name);
                throw;
            }
        }

        public async Task<List<PokemonResponseDto>> GetAllPokemonsAsync(int limit = 20, int offset = 0)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{PokeApiBaseUrl}pokemon?limit={limit}&offset={offset}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error al obtener la lista de Pokémon de la PokeAPI");
                    return new List<PokemonResponseDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var listResponse = JsonSerializer.Deserialize<PokeApiListResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (listResponse == null || !listResponse.Results.Any())
                {
                    return new List<PokemonResponseDto>();
                }

                var pokemonList = new List<PokemonResponseDto>();

                foreach (var pokemonItem in listResponse.Results)
                {
                    try
                    {
                        var pokemonDetail = await ObtenerDetallePokemonDesdeLaApiAsync(pokemonItem.Name);
                        if (pokemonDetail != null)
                        {
                            pokemonList.Add(pokemonDetail);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al obtener detalles del Pokémon {Name}", pokemonItem.Name);
                    }
                }

                _logger.LogInformation("Se obtuvieron {Count} Pokémon de la PokeAPI", pokemonList.Count);
                return pokemonList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de Pokémon");
                throw;
            }
        }

        private string NormalizarNombrePokemon(string name)
        {
            return name.ToLower().Trim().Replace(" ", "-");
        }

        private async Task<PokemonResponseDto?> ObtenerDetallePokemonDesdeLaApiAsync(string name)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{PokeApiBaseUrl}pokemon/{name}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var pokeApiResponse = JsonSerializer.Deserialize<PokeApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pokeApiResponse == null)
                {
                    return null;
                }

                var descripcion = await PokemonApiDescripcionAsync(pokeApiResponse.Species.Url);

                return new PokemonResponseDto
                {
                    Id = pokeApiResponse.Id,
                    Name = pokeApiResponse.Name,
                    Power = pokeApiResponse.Stats.FirstOrDefault(s => s.Stat.Name == "attack")?.Base_stat ?? 0,
                    Description = descripcion,
                    ImageUrl = pokeApiResponse.Sprites.Front_default ?? string.Empty,
                    Category = pokeApiResponse.Types.FirstOrDefault()?.Type.Name ?? string.Empty,
                    Nature = pokeApiResponse.Abilities.FirstOrDefault()?.Ability.Name ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de PokeAPI para {Name}", name);
                return null;
            }
        }

        private PokemonResponseDto MappearDToPokemon(Pokemon pokemon)
        {
            return new PokemonResponseDto
            {
                Id = pokemon.PokeApiId,
                Name = pokemon.Name,
                Power = pokemon.Power,
                Description = pokemon.Description,
                ImageUrl = pokemon.ImageUrl,
                Category = pokemon.Category,
                Nature = pokemon.Nature
            };
        }

        private async Task<Pokemon?> PokemonDeApiPublicaAsync(string name)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{PokeApiBaseUrl}pokemon/{name}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var pokeApiResponse = JsonSerializer.Deserialize<PokeApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pokeApiResponse == null)
                {
                    return null;
                }

                var descripcion = await PokemonApiDescripcionAsync(pokeApiResponse.Species.Url);

                var pokemon = new Pokemon
                {
                    PokeApiId = pokeApiResponse.Id,
                    Name = pokeApiResponse.Name,
                    Power = pokeApiResponse.Stats.FirstOrDefault(s => s.Stat.Name == "attack")?.Base_stat ?? 0,
                    Description = descripcion,
                    ImageUrl = pokeApiResponse.Sprites.Front_default ?? string.Empty,
                    Category = pokeApiResponse.Types.FirstOrDefault()?.Type.Name ?? string.Empty,
                    Nature = pokeApiResponse.Abilities.FirstOrDefault()?.Ability.Name ?? string.Empty
                };

                return pokemon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de PokeAPI para {Name}", name);
                return null;
            }
        }

        private async Task<string> PokemonApiDescripcionAsync(string speciesUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(speciesUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                var content = await response.Content.ReadAsStringAsync();
                var speciesResponse = JsonSerializer.Deserialize<SpeciesResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var englishEntry = speciesResponse?.Flavor_text_entries
                    .FirstOrDefault(f => f.Language.Name == "es");

                return englishEntry?.Flavor_text.Replace("\n", " ").Replace("\f", " ") ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener descripción del Pokémon desde {Url}", speciesUrl);
                return string.Empty;
            }
        }
    }


}
