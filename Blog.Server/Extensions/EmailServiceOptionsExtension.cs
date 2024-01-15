using Blog.Server.Models.Configs;
using VoDA.AspNetCore.Services.Email;

namespace Blog.Server.Extensions
{
    public static class EmailServiceOptionsExtension
    {
        public static void LoadFromConfig(this EmailServiceOptions options, EmailServiceConfigModel configModel)
        {
            options.Email = configModel.Email;
            options.DisplayName = configModel.DisplayName;
            options.Password = configModel.Password;
            options.Host = configModel.Host;
            options.Port = configModel.Port;
            options.EnableSsl = configModel.EnableSsl;
            options.UseDefaultCredentials = configModel.UseDefaultCredentials;
            options.EmailTemplatesFolder = configModel.EmailTemplatesFolder;
        }
    }
}
