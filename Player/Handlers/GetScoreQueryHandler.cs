using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Models;
using TestMediatR1.Player.Queries;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Handlers
{
    public class GetScoreQueryHandler : IRequestHandler<GetScoreQuery, ScoreResponse>
    {
        private readonly MyDbContext _context;

        public GetScoreQueryHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ScoreResponse> Handle(GetScoreQuery request, CancellationToken cancellationToken)
        {
            var player1 = await _context.tblPlayer
                .Include(p => p.Score)
                .FirstOrDefaultAsync(p => p.Username == request.Player1Name, cancellationToken);

            var player2 = await _context.tblPlayer
            .Include(p => p.Score)
            .FirstOrDefaultAsync(p => p.Username == request.Player2Name, cancellationToken);

            if (player1 == null || player2 == null)
            {
                return new ScoreResponse
                {
                    Success = false,
                    Message = "One or both players not found"
                };
            }

            return new ScoreResponse
            {
                Success = true,
                Message = "Player scores found successfully",
                Player1Score = new PlayerScore
                {
                    TotalWins = player1.Score!.TotalWins,
                    TotalLosses = player1.Score!.TotalLosses
                },
                Player2Score = new PlayerScore
                {
                    TotalWins = player2.Score!.TotalWins,
                    TotalLosses = player2.Score!.TotalLosses
                }
            };
        }
    }
}
