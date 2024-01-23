using Blog.Server.Data.Models;
using Blog.Server.Models.Requests;

namespace Blog.Server.Services.PostService
{
    public interface IPostService
    {
        public Task<IEnumerable<string>> SearchTags(string query);
        public IQueryable<PostModel> Search(PostSearchRequestModel requestModel);
        public Task<PostModel> GetPost(int id);
        public IQueryable<PostModel> GetPosts(PageRequestModel pageRequest);
        public Task<PostModel> CreatePost(CreatePostRequestModel requestModel, IFormFileCollection? files);
        public Task<PostModel> UpdatePost(int id, UpdatePostRequestModel requestModel);
        public Task<PostModel> DeletePost(int id);
    }
}
