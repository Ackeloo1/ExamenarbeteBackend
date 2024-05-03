using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Item.Models;
using TestMediatR1.Item.Queries;

namespace TestMediatR1.Item.Handlers
{
    public class GetAllItemIdsHandler : IRequestHandler<GetAllItemIdsQuery, int[]>
    {
        private readonly MyDbContext _context;

        public GetAllItemIdsHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<int[]> Handle(GetAllItemIdsQuery request, CancellationToken cancellationToken)
        {
            var allIds = await _context.tblItem.Select(i => i.Id).ToArrayAsync();
            return allIds;
        }
    }
}
