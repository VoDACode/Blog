namespace Blog.Server.Services.AuthService
{
    public static class AuthServiceExtensions
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
