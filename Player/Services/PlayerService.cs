using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Player.Services
{
    public class PlayerService
    {
        private readonly IMediator _mediator;

        public PlayerService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<PlayerModel>> GetPlayers(List<int> Ids)
        {
            var command = new GetPlayersByIdCommand();
            command.Ids = Ids;
            return await _mediator.Send(command);
        }
    }
}
