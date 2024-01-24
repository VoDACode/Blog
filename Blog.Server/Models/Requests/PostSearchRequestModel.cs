using Microsoft.AspNetCore.Mvc;

namespace Blog.Server.Models.Requests
{
    public class PostSearchRequestModel : PageRequestModel
    {
        [FromQuery(Name = "query")]
        public string? Query { get; set; } = null;
    }
}
