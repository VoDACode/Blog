using System.Security.Claims;

namespace Blog.Server.Extensions
{
    public static class HttpContextExtension
    {
        public static int GetUserId(this HttpContext httpContext)
        {
            var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                throw new Exception("User is not authenticated");
            }

            return int.Parse(userId);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            var userName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (userName is null)
            {
                throw new Exception("User is not authenticated");
            }

            return userName;
        }

        public static bool IsAdmin(this HttpContext httpContext)
        {
            return httpContext.User.IsInRole("admin");
        }
    }
}
