using Blog.Server.Attributes;
using Blog.Server.Exceptions;
using Blog.Server.Models.Requests;
using Blog.Server.Models.Responses;
using Blog.Server.Services.PostService;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Server.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly ILogger logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            this.postService = postService;
            this.logger = logger;
        }

        [AuthorizeAnyType(AllowAnonymous = true)]
        [HttpGet]
        public async Task<IActionResult> Get(PostSearchRequestModel request)
        {
            return await Execute(async () =>
            {
                var posts = postService.GetPosts(request);
                var postModels = posts.Select(p => new PostResponse.View(p));
                var response = await PageResponse<PostResponse.View>.Create(postModels, request);
                return response;
            });
        }

        [AuthorizeAnyType(AllowAnonymous = true)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return await Execute(async () => new PostResponse(await postService.GetPost(id)));
        }

        [HttpGet("search/tags")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            return await Execute(async () =>
            {
                var tags = await postService.SearchTags(query);
                return BaseResponse.Ok(null, tags);
            });
        }

        [AuthorizeAnyType(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CreatePostRequestModel requestModel)
        {
            return await Execute(async () => new PostResponse(await postService.CreatePost(requestModel)));
        }

        [AuthorizeAnyType(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] UpdatePostRequestModel requestModel)
        {
            return await Execute(async () => new PostResponse(await postService.UpdatePost(id, requestModel)));
        }

        [AuthorizeAnyType(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await Execute(async () => new PostResponse(await postService.DeletePost(id)));
        }

        private async Task<IActionResult> Execute(Func<Task<BaseResponse>> func)
        {
            try
            {
                return Ok(await func());
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(BaseResponse.Fail(e.Message));
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(BaseResponse.Fail(e.Message));
            }
            catch (NotFoundException e)
            {
                return NotFound(BaseResponse.Fail(e.Message));
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
