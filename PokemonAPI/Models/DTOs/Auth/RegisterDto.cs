using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.DTOs.Auth
{
    public class RegisterDto
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
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        public string Password { get; set; } = string.Empty;



    }

}
