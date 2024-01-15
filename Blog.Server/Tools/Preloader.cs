using Blog.Server.Data;
using Blog.Server.Data.Models;
using Blog.Server.Models.Configs;
using Blog.Server.Services.HashService;
using Microsoft.Extensions.Options;

namespace Blog.Server.Tools
{
    public static class Preloader
    {
        private static object mutex = new object();
        private static bool isPreloaded = false;
        
        public static void Preload(IServiceProvider serviceProvider)
        {
            if (isPreloaded)
            {
                return;
            }
            lock (mutex)
            {
                CreateFirstUser(serviceProvider);
                isPreloaded = true;
            }
        }
        
        public static void CreateFirstUser(IServiceProvider serviceProvider)
        {
            BlogDbContext dbContext = serviceProvider.GetRequiredService<BlogDbContext>();
            IHashService hashService = serviceProvider.GetRequiredService<IHashService>();
            DefaultUserConfigModel defaultUserConfig = serviceProvider.GetRequiredService<IOptions<DefaultUserConfigModel>>().Value;

            if (dbContext.Users.Any())
            {
                return;
            }

            dbContext.Users.Add(new UserModel
            {
                Username = defaultUserConfig.Username,
                PasswordHash = hashService.Hash(defaultUserConfig.Password),
                Email = defaultUserConfig.Email,
                IsAdmin = true,
                CanPublish = true,
                IsBanned = false
            });

            dbContext.SaveChanges();
        }
    }
}
