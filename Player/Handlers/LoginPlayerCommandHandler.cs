using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.DbContext;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Responses;
using BCrypt.Net;
using TestMediatR1.Services;

namespace TestMediatR1.Player.Handlers
{
    public class LoginPlayerCommandHandler : IRequestHandler<LoginPlayerCommand, LoginResponse>
    {
        private readonly MyDbContext _context;
        private readonly JwtService _jwtService;

        public LoginPlayerCommandHandler(MyDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginPlayerCommand request, CancellationToken cancellationToken)
        {
            var player = await _context.tblPlayer.SingleOrDefaultAsync(p => p.Username == request.Username);
            
            if (player == null)
                return new LoginResponse(false, "");
            
            if (request.Password != player.Password)
                return new LoginResponse(false, "");

            var token = await _jwtService.GenerateAsync(player.Id);

            return new LoginResponse(true, token);
        }
    }
}
