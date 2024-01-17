using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Data.Models
{
    public class TagModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Tag { get; set; }

        public ICollection<PostModel> Posts { get; set; } = new List<PostModel>();
    }
}
