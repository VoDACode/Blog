namespace Blog.Server.Services.FileStorage
{
    public interface IFileStorage
    {
        public Task<Stream> Download(string pathToFile);
        public Task Upload(string pathToFile, Stream fileStream, long fileSize = -1);
        public Task Delete(string pathToFile);
    }
}
