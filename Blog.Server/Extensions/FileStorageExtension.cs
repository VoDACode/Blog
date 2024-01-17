using Blog.Server.Data.Models;
using Blog.Server.Services.FileStorage;

namespace Blog.Server.Extensions
{
    public static class FileStorageExtension
    {
        public static async Task SaveFile(this IFileStorage fileStorage, FileModel model, IFormFile file)
        {
            var fileName = $"{model.Id}{Path.GetExtension(file.FileName)}";
            using (var stream = file.OpenReadStream())
            {
                await fileStorage.Upload(fileName, stream);
            }
            model.Name = file.FileName;
        }

        public static async Task DeleteFile(this IFileStorage fileStorage, FileModel model)
        {
            var fileName = $"{model.Id}{Path.GetExtension(model.Name)}";
            await fileStorage.Delete(fileName);
        }

        public static async Task<Stream> DownloadFile(this IFileStorage fileStorage, FileModel model)
        {
            var fileName = $"{model.Id}{Path.GetExtension(model.Name)}";
            return await fileStorage.Download(fileName);
        }
    }
}
