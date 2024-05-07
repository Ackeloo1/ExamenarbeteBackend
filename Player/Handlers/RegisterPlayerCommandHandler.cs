using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Models;
using TestMediatR1.Player.Responses;
using BCrypt.Net;

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

            var newPlayer = new PlayerModel
            {
                Username = request.Username,
                Password = request.Password,
                Score = request.Score
            };

            _context.tblPlayer.Add(newPlayer);
            await _context.SaveChangesAsync();
            return new RegisterResponse(true, "Player registered successfully.");
        }
    }
}
