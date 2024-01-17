using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Data.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        [EmailAddress]
        [Required]
        public required string Email { get; set; }
        
        public bool IsAdmin { get; set; } = false;
        public bool CanPublish { get; set; } = false;
        public bool IsBanned { get; set; } = false;

        public ICollection<PostModel> Records { get; set; } = new List<PostModel>();
        public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }
}
