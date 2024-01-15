using Blog.Server.Models.Configs;
using Blog.Server.Models.Requests;
using Blog.Server.Models.Responses;
using Blog.Server.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blog.Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly IAuthService authService;
        protected readonly ILogger<AuthController> logger;
        protected readonly SystemConfigModel systemConfig;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IOptions<SystemConfigModel> systemConfig)
        {
            this.authService = authService;
            this.logger = logger;
            this.systemConfig = systemConfig.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (await authService.Login(model.Username, model.Password))
            {
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (!systemConfig.AllowRegistration)
            {
                return BadRequest(BaseResponse.Fail("Registration is disabled"));
            }

            try
            {
                await authService.Register(model.Username, model.Password, model.Email);
                return Ok(BaseResponse.Ok());
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(BaseResponse.Fail(e.Message));
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return StatusCode(500, BaseResponse.Fail("Internal server error"));
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
        {
            if (await authService.ConfirmEmail(token, email))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            authService.Logout();
            return Ok();
        }
    }
}
