using MediatR;
using System.ComponentModel.DataAnnotations;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Commands
{
    public class LoginPlayerCommand : IRequest<LoginResponse>
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
