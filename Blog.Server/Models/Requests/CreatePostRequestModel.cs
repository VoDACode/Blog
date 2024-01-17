using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Models.Requests
{
    public class CreatePostRequestModel
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; } = null!;
        [MaxLength(2048)]
        public string? Content { get; set; }
        public bool IsPublished { get; set; } = false;
        public bool HasComments { get; set; } = false;
        public IEnumerable<string> Tags { get; set; } = new List<string>();
        public IFormFileCollection Files { get; set; } = new FormFileCollection();
    }
}
