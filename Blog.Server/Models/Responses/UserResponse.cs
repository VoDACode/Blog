using Blog.Server.Data.Models;
using Newtonsoft.Json;

namespace Blog.Server.Models.Responses
{
    public class UserResponse : BaseResponse
    {
        public UserResponse(UserModel model, bool showSecureInfo = false) : base(true, new View(model, showSecureInfo))
        {
        }
        public UserResponse(IEnumerable<UserModel> models, bool showSecureInfo = false) : base(true, null)
        {
            Data = models.Select(m => new View(m, showSecureInfo));
        }


        public class View
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public bool IsAdmin { get; set; }
            public bool CanPublish { get; set; }
            public bool IsBanned { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string? Email { get; set; }

            public View(UserModel model, bool showSecureInfo)
            {
                Id = model.Id;
                Username = model.Username;
                IsAdmin = model.IsAdmin;
                CanPublish = model.CanPublish;
                IsBanned = model.IsBanned;
                if (showSecureInfo)
                {
                    Email = model.Email;
                }
            }
        }
    }
}
