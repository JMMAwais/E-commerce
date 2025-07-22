using Microsoft.AspNetCore.Identity;

namespace E_commerce.Model
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
