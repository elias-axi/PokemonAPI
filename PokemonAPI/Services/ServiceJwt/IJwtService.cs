
using Microsoft.AspNetCore.Identity;

namespace PokemonAPI.Services.ServiceJwt
{
    public interface IJwtService
    {
        string GenerateToken(IdentityUser user);

    }
}
