using MediatR;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Item.Commands
{
    public class InsertItemCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Effect { get; set; }
    }
}
