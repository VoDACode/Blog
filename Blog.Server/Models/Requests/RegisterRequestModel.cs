using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Models.Requests
{
    public class RegisterRequestModel
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
    }
}
