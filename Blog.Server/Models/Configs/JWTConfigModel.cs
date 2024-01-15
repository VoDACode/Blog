using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog.Server.Models.Configs
{
    public class JWTConfigModel
    {
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpirationHours { get; set; } = 0;

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }
    }
}
