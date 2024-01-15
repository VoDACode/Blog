namespace Blog.Server.Models.Configs
{
    public class SystemConfigModel
    {
        public bool AllowRegistration { get; set; } = false;
        public bool NeedEmailConfirmation { get; set; } = false;
        public string BaseUrl { get; set; } = null!;
    }
}
