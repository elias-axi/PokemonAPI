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
        /// <param name="name">Nombre del Pokémon a buscar</param>
        /// <returns>Información del Pokémon incluyendo Id, Name, Power, Description, ImageUrl, Category y Nature</returns>
        /// <response code="200">Pokémon encontrado o creado exitosamente</response>
        /// <response code="400">Solicitud inválida - validación de datos fallida</response>
        /// <response code="404">Pokémon no encontrado en la PokeAPI</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(PokemonResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPokemonByName(string name)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
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


    }

}
