using MediatR;
using TestMediatR1.DbContext;
using TestMediatR1.Item.Commands;
using TestMediatR1.Item.Models;

namespace TestMediatR1.Item.Handlers
{
    public class InsertItemCommandHandler : IRequestHandler<InsertItemCommand, int>
    {
        private readonly MyDbContext _context;

        public InsertItemCommandHandler(MyDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(InsertItemCommand request, CancellationToken cancellationToken)
        {
            var newItem = new ItemModel
            {
                Name = request.Name!,
                Effect = request.Effect!
            };

            _context.tblItem.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem.Id;
        }
    }
}
