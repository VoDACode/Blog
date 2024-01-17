using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Data.Models
{
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }
        [Required]
        public required long Size { get; set; }
        [Required]
        public required string ContentType { get; set; }
        [Required]
        public required int PostId { get; set; }
        public PostModel Post { get; set; } = null!;
    }
}
