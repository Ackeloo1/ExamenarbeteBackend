using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Player.Handlers
{
    public class GetPlayersByIdCommandHandler : IRequestHandler<GetPlayersByIdCommand, List<PlayerModel>>
    {
        private readonly MyDbContext _context;

        public GetPlayersByIdCommandHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerModel>> Handle(GetPlayersByIdCommand request, CancellationToken cancellationToken)
        {
            var players = await _context.tblPlayer.Where(i => request.Ids.Contains(i.Id)).ToListAsync();
            return players;
        }
    }
}
