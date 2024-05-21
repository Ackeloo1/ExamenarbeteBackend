using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Models;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Handlers
{
    public class RegisterPlayerCommandHandler : IRequestHandler<RegisterPlayerCommand, RegisterResponse>
    {
        private readonly MyDbContext _context;

        public RegisterPlayerCommandHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResponse> Handle(RegisterPlayerCommand request, CancellationToken cancellationToken)
        {
            var existingPlayer = await _context.tblPlayer.SingleOrDefaultAsync(p => p.Username == request.Username);

            if (existingPlayer != null)
            {
                return new RegisterResponse(false, "Username already exists.");
            }

            var hashedPassword = HashPassword(request.Password);

            var playerScore = new PlayerScore
            {
                Score = 0,
                TotalWins = 0,
                TotalLosses = 0
            };

            //await _context.tblScore.AddAsync(playerScore);

            Console.WriteLine(playerScore.ScoreId);

            var player = new PlayerModel
            {
                Username = request.Username,
                Password = hashedPassword,
                Score = playerScore
            };

            await _context.tblPlayer.AddAsync(player);
            await _context.SaveChangesAsync();

            return new RegisterResponse(true, "Player registered successfully.");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
