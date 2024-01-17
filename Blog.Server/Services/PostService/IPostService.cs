using Blog.Server.Data.Models;
using Blog.Server.Models.Requests;

namespace Blog.Server.Services.PostService
{
    public interface IPostService
    {
        public Task<PostModel> GetPost(int id);
        public Task<IEnumerable<PostModel>> GetPosts(PageRequestModel pageRequest);
        public Task<PostModel> CreatePost(CreatePostRequestModel requestModel);
        public Task<PostModel> UpdatePost(int id, UpdatePostRequestModel requestModel);
        public Task<PostModel> DeletePost(int id);
    }
}
