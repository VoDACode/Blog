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
        public async Task<IActionResult> Get(PageRequestModel request)
        {
            try
            {
                var post = await postService.GetPosts(request);
                return Ok(new PostResponse(post));
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [AuthorizeAnyType(AllowAnonymous = true)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var post = await postService.GetPost(id);
                return Ok(new PostResponse(post));
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

        [AuthorizeAnyType(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CreatePostRequestModel requestModel)
        {
            try
            {
                var post = await postService.CreatePost(requestModel);
                return Ok(new PostResponse(post));
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(BaseResponse.Fail(e.Message));
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [AuthorizeAnyType(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdatePostRequestModel requestModel)
        {
            try
            {
                var post = await postService.UpdatePost(id, requestModel);
                return Ok(new PostResponse(post));
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

        [AuthorizeAnyType(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var post = await postService.DeletePost(id);
                return Ok(new PostResponse(post));
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
