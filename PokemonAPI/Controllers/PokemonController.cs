using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonAPI.Models.DTOs.Pokemon;
using PokemonAPI.Services.ServicesPokemon;

namespace PokemonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(IPokemonService pokemonService, ILogger<PokemonController> logger)
        {
            _pokemonService = pokemonService;
            _logger = logger;
        }

        /// <summary>
        /// Busca un Pokémon por nombre en la base de datos. Si no existe, lo obtiene de la PokeAPI y lo guarda.
        /// </summary>
        /// <param name="name">Nombre del Pokémon a buscar (puede contener espacios que se convertirán automáticamente en guiones)</param>
        /// <returns>Información del Pokémon incluyendo Id, Name, Power, Description, ImageUrl, Category y Nature</returns>
        /// <response code="200">Pokémon encontrado o creado exitosamente</response>
        /// <response code="400">Solicitud inválida - validación de datos fallida</response>
        /// <response code="404">Pokémon no encontrado en la PokeAPI</response>
        /// <response code="500">Error interno del servidor</response>
        /// <remarks>
        /// Ejemplos de nombres válidos:
        /// - pikachu
        /// - mr mime (o mr-mime)
        /// - tapu koko (o tapu-koko)
        /// </remarks>
        [HttpGet(("{name}"))]
        [ProducesResponseType(typeof(PokemonResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPokemonByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { message = "El nombre del Pokémon es requerido" });
                }

                var pokemon = await _pokemonService.GetPokemonByNameAsync(name);

                if (pokemon == null)
                {
                    return NotFound(new { message = $"Pokémon '{name}' no encontrado" });
                }

                return Ok(pokemon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el Pokémon {Name}", name);
                return StatusCode(500, new { message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Obtiene una lista de Pokémon desde la PokeAPI pública sin guardarlos en la base de datos.
        /// </summary>
        /// <param name="limit">Número máximo de Pokémon a obtener (por defecto 20)</param>
        /// <param name="offset">Número de Pokémon a omitir para paginación (por defecto 0)</param>
        /// <returns>Lista de Pokémon con sus detalles completos</returns>
        /// <response code="200">Lista de Pokémon obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        /// <remarks>
        /// Este endpoint consulta directamente la PokeAPI pública y no guarda los datos en la base de datos.
        /// Ejemplo de uso:
        /// - GET /api/pokemon?limit=10&amp;offset=0 (obtiene los primeros 10 Pokémon)
        /// - GET /api/pokemon?limit=20&amp;offset=20 (obtiene los siguientes 20 Pokémon)
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<PokemonResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPokemons([FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new { message = "El límite debe estar entre 1 y 100" });
                }

                if (offset < 0)
                {
                    return BadRequest(new { message = "El offset no puede ser negativo" });
                }

                var pokemons = await _pokemonService.GetAllPokemonsAsync(limit, offset);
                return Ok(pokemons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de Pokémon");
                return StatusCode(500, new { message = "Error al procesar la solicitud" });
            }
        }
    }
}
