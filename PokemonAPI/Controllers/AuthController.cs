using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PokemonAPI.Models.DTOs.Auth;
using PokemonAPI.Services.ServiceJwt;

namespace PokemonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="registerDto">Datos del usuario a registrar</param>
        /// <returns>El usuario creado</returns>
        /// <response code="200">Usuario creado exitosamente</response>
        /// <response code="400">Los datos proporcionados son inválidos</response>
        /// <response code="409">El email ya está registrado</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Conflict(new
                {
                    message = "El email ya está registrado",
                    code = "EMAIL_ALREADY_EXISTS"
                });
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = $"Error al crear el usuario: {errors}"
                });
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Usuario registrado exitosamente: {Email}", user.Email);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
            });
        }
        /// <summary>
        /// Inicia sesión de un usuario en el sistema
        /// </summary>
        /// <param name="loginDto">Credenciales de inicio de sesión</param>
        /// <returns>Token JWT y datos del usuario</returns>
        /// <response code="200">Inicio de sesión exitoso</response>
        /// <response code="400">Los datos proporcionados son inválidos</response>
        /// <response code="401">Credenciales incorrectas</response>
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Datos de inicio de sesión inválidos"
                });
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Correo electrónico o contraseña incorrectos"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Correo electrónico o contraseña incorrectos"
                });
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Usuario inició sesión exitosamente: {Email}", user.Email);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Inicio de sesión exitoso",
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,

                }
            });
        }
    }

}
