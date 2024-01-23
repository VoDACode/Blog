using Blog.Server.Attributes;
using Blog.Server.Data;
using Blog.Server.Enums;
using Blog.Server.Extensions;
using Blog.Server.Models.Requests;
using Blog.Server.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected readonly BlogDbContext dbContext;

        public UserController(BlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any)]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            return await Get(HttpContext.GetUserId());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var showSecureInfo = HttpContext.IsAdmin() || HttpContext.GetUserId() == id;
                var user = await dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(BaseResponse.Fail($"User {id} not found"));
                }
                return Ok(new UserResponse(user, showSecureInfo));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any)]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateRequest request)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(HttpContext.GetUserId());
                if (user == null)
                {
                    return NotFound(BaseResponse.Fail($"User {HttpContext.GetUserId()} not found"));
                }

                user.Email = request.Email;
                user.Username = request.Username;

                await dbContext.SaveChangesAsync();

                return Ok(new UserResponse(user, true));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any, Roles = "admin")]
        [HttpPut("{id}/ban")]
        public async Task<IActionResult> Ban(int id)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(BaseResponse.Fail($"User {id} not found"));
                }

                if(user.Id == HttpContext.GetUserId())
                {
                    return BadRequest(BaseResponse.Fail($"User {id} cannot ban himself"));
                }

                user.IsBanned = !user.IsBanned;

                await dbContext.SaveChangesAsync();

                return Ok(new UserResponse(user, true));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AuthorizeAnyType(Type = AuthorizeType.Any, Roles = "admin")]
        [HttpPut("{id}/can-publish")]
        public async Task<IActionResult> CanPublish(int id)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(BaseResponse.Fail($"User {id} not found"));
                }

                if(user.Id == HttpContext.GetUserId())
                {
                    return BadRequest(BaseResponse.Fail($"User {id} cannot change his own permissions"));
                }

                user.CanPublish = !user.CanPublish;

                await dbContext.SaveChangesAsync();

                return Ok(new UserResponse(user, true));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
