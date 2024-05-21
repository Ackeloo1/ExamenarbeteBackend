using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Handlers
{
    public class AddScoreCommandHandler : IRequestHandler<AddScoreCommand, ScoreResponse>
    {
        private readonly MyDbContext _context;

        public AddScoreCommandHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<ScoreResponse> Handle(AddScoreCommand request, CancellationToken cancellationToken)
        {
            var winner = await _context.tblPlayer
                .Include(p => p.Score)
                .FirstOrDefaultAsync(p => p.Username == request.WinnerName);
            var loser = await _context.tblPlayer
                .Include(p => p.Score)
                .FirstOrDefaultAsync(p => p.Username == request.LoserName);

            if (winner == null || loser == null)
            {
                return new ScoreResponse
                {
                    Success = false,
                    Message = "Winner or loser not found error"
                };
            }

            winner.Score!.TotalWins++;
            loser.Score!.TotalLosses++;

            try
            {
                await _context.SaveChangesAsync();

                return new ScoreResponse
                {
                    Success = true,
                    Message = "Player scores updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ScoreResponse
                {
                    Success = false,
                    Message = $"Error updating player scores: {ex.Message}"
                };
            }
        }
    }
}
