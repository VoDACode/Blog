using Blog.Server.Attributes;
using Blog.Server.Data;
using Blog.Server.Enums;
using Blog.Server.Extensions;
using Blog.Server.Models.Requests;
using Blog.Server.Models.Responses;
using Blog.Server.Services.HashService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected readonly BlogDbContext dbContext;
        protected readonly IHashService hashService;

        public UserController(BlogDbContext dbContext, IHashService hashService)
        {
            this.dbContext = dbContext;
            this.hashService = hashService;
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

                if(await dbContext.Users.AnyAsync(x => x.Username == request.Username && x.Id != user.Id))
                {
                    return BadRequest(BaseResponse.Fail($"Username {request.Username} is already taken"));
                }

                if(await dbContext.Users.AnyAsync(x => x.Email == request.Email && x.Id != user.Id))
                {
                    return BadRequest(BaseResponse.Fail($"Email {request.Email} is already taken"));
                }

                if(!string.IsNullOrEmpty(request.NewPassword))
                {
                    if (string.IsNullOrEmpty(request.OldPassword))
                    {
                        return BadRequest(BaseResponse.Fail($"Old password is required"));
                    }

                    if(request.NewPassword == request.OldPassword)
                    {
                        return BadRequest(BaseResponse.Fail($"New password cannot be the same as old password"));
                    }

                    if(!hashService.Verify(request.OldPassword, user.PasswordHash))
                    {
                        return BadRequest(BaseResponse.Fail($"Old password is incorrect"));
                    }

                    user.PasswordHash = hashService.Hash(request.NewPassword);
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
