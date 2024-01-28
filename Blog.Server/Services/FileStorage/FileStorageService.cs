using Microsoft.Extensions.Options;

namespace Blog.Server.Services.FileStorage
{
    public class FileStorageService : IFileStorage
    {
        protected readonly FileStorageServiceConfig configuration;
        private string RootPath => Path.IsPathRooted(configuration.StoragePath) ? configuration.StoragePath : Path.Combine(Directory.GetCurrentDirectory(), configuration.StoragePath);

        private string GetFullPath(string filename)
        {
            return Path.Combine(RootPath, filename);
        }

        public FileStorageService(IOptions<FileStorageServiceConfig> configuration)
        {
            this.configuration = configuration.Value;

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
        }

        public Task Delete(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (!File.Exists(GetFullPath(filename)))
            {
                return Task.CompletedTask;
            }

            string? dir = Path.GetDirectoryName(GetFullPath(filename));

            if (dir == null)
            {
                throw new InvalidOperationException("Directory is null");
            }

            File.Delete(GetFullPath(filename));

            if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
            {
                Directory.Delete(dir);
            }

            return Task.CompletedTask;
        }

        public Task<Stream> Download(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (!File.Exists(GetFullPath(filename)))
            {
                throw new FileNotFoundException("File not found", filename);
            }

            return Task.FromResult(new FileStream(GetFullPath(filename), FileMode.Open, FileAccess.Read) as Stream);
        }

        public async Task Upload(string filename, Stream fileStream, long fileSize = -1)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            string[] pathParts = filename.Split(Path.DirectorySeparatorChar);
            string fileFullPath = RootPath;
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                fileFullPath = Path.Combine(fileFullPath, pathParts[i]);
                if (!Directory.Exists(fileFullPath))
                {
                    Directory.CreateDirectory(fileFullPath);
                }
            }
            fileFullPath = Path.Combine(fileFullPath, pathParts.Last());

            using (var fs = new FileStream(fileFullPath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, 24 * 1024))
            {
                await fileStream.CopyToAsync(fs);
            }
        }

        public Task<long> Size(string pathToFile)
        {
            if (pathToFile == null)
            {
                throw new ArgumentNullException("pathToFile");
            }

            if (!File.Exists(GetFullPath(pathToFile)))
            {
                throw new FileNotFoundException("File not found", pathToFile);
            }

            return Task.FromResult(new FileInfo(GetFullPath(pathToFile)).Length);
        }
    }
}
