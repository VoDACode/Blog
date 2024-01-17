using Blog.Server.Data;
using Blog.Server.Data.Models;
using Blog.Server.Models.Configs;
using Blog.Server.Services.HashService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using VoDA.AspNetCore.Services.Email;

namespace Blog.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        protected readonly BlogDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IHashService hashService;
        protected readonly AuthServiceConfigModel authConfig;
        protected readonly JWTConfigModel jwtConfig;
        protected readonly SystemConfigModel systemConfig;
        protected readonly IMemoryCache cache;
        protected readonly IEmailService emailService;

        public AuthService(BlogDbContext context,
                           IHttpContextAccessor httpContextAccessor,
                           IOptions<JWTConfigModel> jwtConfig,
                           IOptions<SystemConfigModel> systemConfig,
                           IOptions<AuthServiceConfigModel> authConfig,
                           IHashService hashService,
                           IMemoryCache cache,
                           IEmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            this.jwtConfig = jwtConfig.Value;
            this.systemConfig = systemConfig.Value;
            this.authConfig = authConfig.Value;
            this.hashService = hashService;
            this.cache = cache;
            this.emailService = emailService;
        }

        public async Task<bool> ConfirmEmail(string token, string email)
        {
            if(!cache.TryGetValue($"email-confirmation-{email}", out string? cachedUser))
            {
                return false;
            }

            if(cachedUser is null)
            {
                return false;
            }

            var user = JsonSerializer.Deserialize<UserModel>(cachedUser);
            if(user == null)
            {
                return false;
            }

            cache.Remove($"email-confirmation-{email}");
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return false;
            }

            if (hashService.Verify(password, user.PasswordHash))
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"),
                new Claim(ClaimTypes.Role, user.CanPublish ? "publisher" : "user"),
                new Claim(ClaimTypes.Role, user.IsBanned ? "banned" : "user"),
            };

            var secretKey = jwtConfig.GetSymmetricSecurityKey();
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(jwtConfig.ExpirationHours),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(".VoDACode.Authorize", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(jwtConfig.ExpirationHours)
            });
            return true;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(".VoDACode.Authorize");
        }

        public async Task<bool> Register(string username, string password, string email)
        {
            if(cache.TryGetValue($"email-confirmation-{email}", out string? cachedUser))
            {
                throw new BadHttpRequestException("Email already exists");
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new BadHttpRequestException("Email already exists");
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new BadHttpRequestException("Username already exists");
            }

            var user = new UserModel
            {
                Username = username,
                PasswordHash = hashService.Hash(password),
                Email = email,
                CanPublish = false,
                IsAdmin = false,
                IsBanned = false
            };

            if(!systemConfig.NeedEmailConfirmation)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                byte[] confirmCodeBytes = new byte[authConfig.ConfirmationCodeByteSize];
                using (var generator = RandomNumberGenerator.Create())
                {
                    generator.GetBytes(confirmCodeBytes);
                }
                string confirmCode = Convert.ToBase64String(confirmCodeBytes);
                cache.Set($"email-confirmation-{user.Email}", JsonSerializer.Serialize(user), TimeSpan.FromHours(1));

                await emailService.SendEmailUseTemplate(user.Email, "ConfirmEmail", new Dictionary<string, string>
                {
                    { "username", user.Username },
                    { "link", $"{systemConfig.BaseUrl}/api/auth/confirm-email?token={confirmCode}&email={user.Email}" }
                });
            }

            return true;
        }
    }
}
