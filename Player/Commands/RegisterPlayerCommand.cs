using MediatR;
using System.ComponentModel.DataAnnotations;
using TestMediatR1.Player.Responses;

namespace TestMediatR1.Player.Commands
{
    public class RegisterPlayerCommand : IRequest<RegisterResponse>
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } 

        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{5,}$", ErrorMessage = "Weak")]
        [Required]
        public string Password { get; set; }
    }
}
