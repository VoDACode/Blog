using Blog.Server.Data.Models;

namespace Blog.Server.Models.Responses
{
    public class PostResponse : BaseResponse
    {
        public PostResponse(PostModel model) : base(true, new View(model))
        {
        }
        public PostResponse(IEnumerable<PostModel> models) : base(true, null)
        {
            Data = models.Select(m => new View(m)).ToList();
        }

        public PostResponse(IQueryable<PostModel> models) : base(true, null)
        {
            Data = models.Select(m => new View(m)).ToList();
        }


        public class View
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool HasComments { get; set; }
            public bool IsPublished { get; set; }
            public string Author { get; set; } = null!;
            public IEnumerable<string> Tags { get; set; } = null!;
            public IEnumerable<FileResponse.View> Files { get; set; } = null!;

            public View(PostModel model)
            {
                Id = model.Id;
                Title = model.Title;
                Content = model.Content ?? "";
                CreatedAt = model.CreatedAt;
                UpdatedAt = model.UpdatedAt;
                HasComments = model.HasComments;
                IsPublished = model.IsPublished;
                Author = model.Author.Username;
                Tags = model.Tags.Select(t => t.Tag);
                Files = model.Files.Select(f => new FileResponse.View(f));
            }
        }
    }
}
