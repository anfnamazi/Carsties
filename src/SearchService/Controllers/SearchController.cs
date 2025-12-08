using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.searchTerm))
            {
                query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(i => i.Ascending(i => i.Make)),
                "new" => query.Sort(i => i.Ascending(i => i.CreatedAt)),
                _ => query.Sort(i => i.Ascending(i => i.AuctionEnd)),
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(i => i.AuctionEnd > DateTime.UtcNow),
                "endingSoon" => query.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6)
                    && i.AuctionEnd < DateTime.UtcNow),
                _ => query.Match(i => i.AuctionEnd < DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(i => i.Seller.Contains(searchParams.Seller));
            }

            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(i => i.Winner.Contains(searchParams.Winner));
            }

            query.PageNumber(searchParams.pageNumber);
            query.PageSize(searchParams.pageSize);

            var result = await query.ExecuteAsync();
            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount,
            });
        }
    }
}
