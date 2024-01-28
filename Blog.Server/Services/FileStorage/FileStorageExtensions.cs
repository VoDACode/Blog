namespace Blog.Server.Services.FileStorage
{
    public static class FileStorageExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services, Action<FileStorageServiceConfig> options)
        {
            services.AddScoped<IFileStorage, FileStorageService>();
            services.Configure(options);
            return services;
        }
    }
}
