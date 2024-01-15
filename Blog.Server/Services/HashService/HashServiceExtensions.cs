namespace Blog.Server.Services.HashService
{
    public static class HashServiceExtensions
    {
        public static IServiceCollection AddHashService(this IServiceCollection services)
        {
            services.AddSingleton<IHashService, HashService>();
            return services;
        }
    }
}
