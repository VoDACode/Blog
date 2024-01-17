namespace Blog.Server.Services.FileStorage
{
    public static class FileStorageExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services, Action<FileStorageServiceConfig> options)
        {
            services.AddSingleton<IFileStorage, FileStorageService>();
            services.Configure(options);
            return services;
        }
    }
}
