using MediatR;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Commands
{
    public class AddScoreCommand : IRequest<ScoreResponse>
    {
        public string? WinnerName { get; set; }
        public string? LoserName { get; set; }
    }
}
