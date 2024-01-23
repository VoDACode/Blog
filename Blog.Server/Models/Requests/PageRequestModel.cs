using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Models.Requests
{
    public class PageRequestModel
    {
        [FromQuery(Name = "page")]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        [FromQuery(Name = "pageSize")]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
    }
}
