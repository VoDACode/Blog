using Blog.Server.Data.Models;

namespace Blog.Server.Models.Responses
{
    public class FileResponse : BaseResponse
    {
        protected FileResponse(FileModel model) : base(true, new View(model))
        {
        }

        protected FileResponse(IEnumerable<FileModel> models) : base(true, null)
        {
            Data = models.Select(m => new View(m)).ToList();
        }

        public class View
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public long Size { get; set; }
            public string ContentType { get; set; }
            public int PostId { get; set; }

            public View(FileModel model)
            {
                Id = model.Id;
                Name = model.Name;
                Size = model.Size;
                ContentType = model.ContentType;
                PostId = model.PostId;
            }
        }
    }
}
