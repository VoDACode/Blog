using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Data.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(1024)]
        public string Content { get; set; } = null!;
        [Required]
        public int AuthorId { get; set; }
        public UserModel Author { get; set; } = null!;
        [Required]
        public int RecordId { get; set; }
        public RecordModel Record { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
