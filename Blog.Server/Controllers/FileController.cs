using Blog.Server.Attributes;
using Blog.Server.Data;
using Blog.Server.Enums;
using Blog.Server.Extensions;
using Blog.Server.Models.Responses;
using Blog.Server.Services.FileStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Server.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorage fileStorage;
        private readonly ILogger logger;
        protected readonly BlogDbContext dbContext;

        public FileController(IFileStorage fileStorage, ILogger<FileController> logger, BlogDbContext dbContext)
        {
            this.fileStorage = fileStorage;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [AuthorizeAnyType(AllowAnonymous = true)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var file = await dbContext.Files
                .Include(f => f.Post)
                .FirstOrDefaultAsync(f => f.Id == id);

            if(file is null || (!file.Post.IsPublished && !HttpContext.IsAdmin()))
            {
                return NotFound(BaseResponse.Fail($"File {id} not found"));
            }

            var stream = await fileStorage.DownloadFile(file);

            return File(stream, file.ContentType, file.Name, true);
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var file = await dbContext.Files
                .Include(f => f.Post)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file is null || !HttpContext.IsAdmin())
            {
                return NotFound(BaseResponse.Fail($"File {id} not found"));
            }

            if (file.Post.AuthorId != HttpContext.GetUserId())
            {
                return Unauthorized(BaseResponse.Fail($"User {HttpContext.GetUserId()} is not allowed to delete file {id}"));
            }

            try
            {
                dbContext.Files.Remove(file);
                await dbContext.SaveChangesAsync();
                await fileStorage.DeleteFile(file);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to delete file {id}");
                return BadRequest(BaseResponse.Fail($"Failed to delete file {id}"));
            }

            return Ok(BaseResponse.Ok());
        }
    }
}
