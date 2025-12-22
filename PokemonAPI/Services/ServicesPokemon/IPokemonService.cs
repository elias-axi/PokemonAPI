using PokemonAPI.Models.DTOs.Pokemon;

namespace PokemonAPI.Services.ServicesPokemon
{
    public interface IPokemonService
    {
        Task<PokemonResponseDto?> GetPokemonByNameAsync(string name);
        Task<List<PokemonResponseDto>> GetAllPokemonsAsync(int limit = 20, int offset = 0);
    }
}
