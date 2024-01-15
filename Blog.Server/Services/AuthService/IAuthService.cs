namespace Blog.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<bool> Login(string username, string password);
        Task<bool> Register(string username, string password, string email);
        Task<bool> ConfirmEmail(string token, string email);
        void Logout();
    }
}
