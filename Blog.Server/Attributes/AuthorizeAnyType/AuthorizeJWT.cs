using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Blog.Server.Attributes.AuthorizeAnyType
{
    public class AuthorizeJWT : IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext http = context.HttpContext;

            string? jwt = http.Request.Headers["Authorization"];

            if (jwt is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Authorize(context, jwt);
        }

        protected void Authorize(AuthorizationFilterContext context, string token)
        {
            HttpContext http = context.HttpContext;

            var configs = http.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration ?? throw new ArgumentNullException("IConfiguration is null");
            var jwtConfig = configs.GetSection("JWT") ?? throw new ArgumentNullException("JWT config is null");
            var jwtSecret = jwtConfig["SecretKey"] ?? throw new ArgumentNullException("JWT secret is null");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            ValidateToken(context, validationParameters, token);
        }

        private void ValidateToken(AuthorizationFilterContext context, TokenValidationParameters validationParameters, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                if (principal is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var identity = principal.Identity as ClaimsIdentity;

                if (identity is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var exp = identity.FindFirst("exp");

                if (exp is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var expDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp.Value));

                if (expDate < DateTimeOffset.UtcNow)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                context.HttpContext.User = principal;
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
