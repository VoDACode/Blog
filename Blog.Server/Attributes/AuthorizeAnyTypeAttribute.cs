using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Blog.Server.Enums;
using Blog.Server.Attributes.AuthorizeAnyType;

namespace Blog.Server.Attributes
{
    public class AuthorizeAnyTypeAttribute : Attribute, IAuthorizationFilter
    {
        public AuthorizeType Type { get; set; } = AuthorizeType.Any;
        public bool AllowAnonymous { get; set; } = false;
        public string? Roles { get; set; } = null;

        private int intType => (int)Type;
        private string[]? roles => Roles?.Split(',');

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext? http = context.HttpContext;

            if (http is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool isCookie = http.Request.Cookies.Any(p => p.Key == ".VoDACode.Authorize");
            bool isJWT = http.Request.Headers.Any(p => p.Key == "Authorization");

            IAuthorizationFilter? handeled = isCookie && ValidateType(AuthorizeType.Cookie) ? new AuthorizeCookie() :
                                             isJWT && ValidateType(AuthorizeType.JWT) ? new AuthorizeJWT() :
                                             null;

            if (handeled is null)
            {
                if(AllowAnonymous)
                {
                    context.Result = null;
                    return;
                }
                context.Result = new UnauthorizedResult();
                return;
            }

            handeled.OnAuthorization(context);

            if (context.Result is null && Roles is not null)
            {
                var userRoles = context.HttpContext.User.FindFirst(ClaimTypes.Role);
                if (userRoles != null)
                {
                    var parsedRole = userRoles.Value.Split(',');

                    foreach (var role in parsedRole)
                    {
                        if (roles?.Contains(role) == true)
                        {
                            return;
                        }
                    }
                    context.Result = new ForbidResult();
                    return;
                }
            }

            if (this.AllowAnonymous)
            {
                context.Result = null;
            }
        }

        bool ValidateType(AuthorizeType type)
        {
            if ((intType & (int)AuthorizeType.Any) == (int)AuthorizeType.Any)
                return true;
            return (intType & (int)type) == (int)type;
        }
    }
}
