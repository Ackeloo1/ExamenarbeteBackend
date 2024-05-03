using MediatR;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Player.Commands
{
    public class GetPlayersByIdCommand : IRequest<List<PlayerModel>>
    {
        public List<int> Ids { get; set; }
    }
}
