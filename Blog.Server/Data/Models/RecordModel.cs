using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Data.Models
{
    public class RecordModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(128)]
        public required string Title { get; set; }
        [MaxLength(2048)]
        public string? Content { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public UserModel Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? PublishedAt { get; set; } = null;
   
        public bool IsPublished { get; set; } = false;
        public bool HasComments { get; set; } = false;

        public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
        public ICollection<FileModel> Files { get; set; } = new List<FileModel>();
        public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }
}
