using Blog.Server.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Blog.Server.Models.Responses
{
    public class PageResponse<T> : BaseResponse
    {
        public PageResponse(IEnumerable<T> data, PageRequestModel page, int totalItemsCount, int totalPagesCount)
            : base(true, new View(data, page, totalItemsCount, totalPagesCount))
        {
        }

        public static async Task<PageResponse<T>> Create(IQueryable<T> data, PageRequestModel page)
        {
            int totalItemsCount = await data.CountAsync();
            int totalPagesCount = (int)Math.Ceiling(totalItemsCount / (double)page.PageSize);
            IEnumerable<T> items = await data
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToArrayAsync();
            return new PageResponse<T>(items, page, totalItemsCount, totalPagesCount);
        }

        class View
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPagesCount { get; set; }
            public int TotalItemsCount { get; set; }
            public IEnumerable<T> Items { get; set; } = new List<T>();
            public View(IEnumerable<T> data, PageRequestModel page, int totalItemsCount, int totalPagesCount)
            {
                PageNumber = page.PageNumber;
                PageSize = page.PageSize;
                TotalItemsCount = totalItemsCount;
                TotalPagesCount = totalPagesCount;
                Items = data;
            }
        }
    }
}
