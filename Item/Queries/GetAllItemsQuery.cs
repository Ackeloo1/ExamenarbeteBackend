using MediatR;
using TestMediatR1.Item.Models;

namespace TestMediatR1.Item.Queries
{
    public class GetAllItemsQuery : IRequest<List<ItemModel>>
    {

    }
}
