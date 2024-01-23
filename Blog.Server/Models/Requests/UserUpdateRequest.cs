using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Models.Requests
{
    public class UserUpdateRequest
    {
        [Required]
        [MaxLength(64)]
        public string Username { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
