using MediatR;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Item.Models;
using TestMediatR1.Item.Queries;

namespace TestMediatR1.Item.Handlers
{
    public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, List<ItemModel>>
    {
        private readonly MyDbContext _context;

        public GetAllItemsHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemModel>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var allItems = await _context.tblItem.ToListAsync();
            return allItems;
        }
    }
}
