namespace Blog.Server.Services.HashService
{
    public interface IHashService
    {
        string Hash(string data);
        bool Verify(string data, string hash);
    }
}
