using MediatR;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Queries
{
    public class GetScoreQuery : IRequest<ScoreResponse>
    {
        public string Player1Name { get; set; }
        public string Player2Name { get; set;}
    }
}
