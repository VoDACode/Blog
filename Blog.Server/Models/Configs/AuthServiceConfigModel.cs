namespace Blog.Server.Models.Configs
{
    public class AuthServiceConfigModel
    {
        public int SaltByteSize { get; set; }
        public int HashByteSize { get; set; }
        public int HasingIterationsCount { get; set; }
        public int ConfirmationCodeByteSize { get; set; }
    }
}
