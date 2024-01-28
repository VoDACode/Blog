using Blog.Server.Attributes;
using Blog.Server.Data;
using Blog.Server.Data.Models;
using Blog.Server.Enums;
using Blog.Server.Extensions;
using Blog.Server.Models.Responses;
using Blog.Server.Services.FileStorage;
using Blog.Server.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Blog.Server.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private const long MaxFileSize = 20L * 1024L * 1024L * 1024L;
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

            if (file is null || (!file.Post.IsPublished && !HttpContext.IsAdmin()))
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

        [AuthorizeAnyType(Type = AuthorizeType.User)]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromQuery] int postId)
        {
            var post = await dbContext.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);
            if (post is null)
            {
                return NotFound(BaseResponse.Fail($"Post {postId} not found"));
            }

            if (post.AuthorId != HttpContext.GetUserId())
            {
                return Unauthorized(BaseResponse.Fail($"User {HttpContext.GetUserId()} is not allowed to upload files to post {postId}"));
            }

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest(BaseResponse.Fail("Not a multipart request"));
            }

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
            var reader = new MultipartReader(boundary, Request.Body);

            var section = await reader.ReadNextSectionAsync();

            if (section == null)
            {
                return BadRequest(BaseResponse.Fail("No sections in multipart defined"));
            }

            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
            {
                return BadRequest(BaseResponse.Fail("No content disposition in multipart defined"));
            }

            var fileName = contentDisposition.FileNameStar.ToString();
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = contentDisposition.FileName.ToString();
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest(BaseResponse.Fail("No filename defined"));
            }

            using (var fileStream = section.Body)
            {
                var file = new FileModel
                {
                    Name = fileName,
                    ContentType = section.ContentType ?? "application/octet-stream",
                    PostId = postId,
                    Post = post,
                    Size = section.Body.Length,
                };

                file = (await dbContext.Files.AddAsync(file)).Entity;
                await dbContext.SaveChangesAsync();

                try
                {
                    await fileStorage.SaveFile(file, fileStream);
                    file.Size = await fileStorage.Size(file);
                    await dbContext.SaveChangesAsync();
                    return Ok(new FileResponse(file));
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Failed to save file {fileName}");
                    dbContext.Files.Remove(file);
                    await dbContext.SaveChangesAsync();
                    return BadRequest(BaseResponse.Fail($"Failed to save file {fileName}"));
                }
            }
        }
    }
}
