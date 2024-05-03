using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestMediatR1.Item.Queries;
using TestMediatR1.Item.Models;
using TestMediatR1.Item.Commands;
using Microsoft.AspNetCore.Authorization;

namespace TestMediatR1.Controllers
{
    [Route("ShotgunRoulette/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("GetItems")]
        public async Task<ActionResult<List<ItemModel>>> GetAllItems()
        {
            var query = new GetAllItemsQuery();
            var items = await _mediator.Send(query);
            return Ok(items);
        }

        [HttpPost("InsertItem")]
        public async Task<ActionResult<ItemModel>> InsertItem([FromBody] InsertItemCommand command)
        {
            var newItem = await _mediator.Send(command);
            return Ok(newItem);
        }
    }
}
