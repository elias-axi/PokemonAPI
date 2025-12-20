using PokemonAPI.Models.DTOs.Pokemon;

namespace PokemonAPI.Services.ServicesPokemon
{
    public interface IPokemonService
    {
        Task<PokemonResponseDto?> GetPokemonByNameAsync(string name);
    }
}
