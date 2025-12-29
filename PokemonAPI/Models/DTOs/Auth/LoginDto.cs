using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.DTOs.Auth
{
    public class LoginDto
    {
        [DefaultValue("string")]
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [MinLength(16, ErrorMessage = "El correo electrónico debe tener al menos 16 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "El correo electrónico debe tener un formato válido con @ y dominio")]
        public string Email { get; set; } = string.Empty;

        [DefaultValue("string")]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;


    }

}
