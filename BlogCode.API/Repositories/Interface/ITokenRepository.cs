using Microsoft.AspNetCore.Identity;

namespace BlogCode.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateToken(IdentityUser user, List<string> roles);
    }
}
