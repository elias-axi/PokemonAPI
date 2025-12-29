using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.DTOs.Pokemon
{
    public class PokemonRequest
    {
        [DefaultValue("string")]
        [Required(ErrorMessage = "El nombre del Pokémon es requerido")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z\-]+$", ErrorMessage = "El nombre solo puede contener letras y guiones")]
        public string Name { get; set; } = string.Empty;
    }

}
