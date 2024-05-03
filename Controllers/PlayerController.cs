using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Models;
using TestMediatR1.Player.Responses;
using TestMediatR1.Services;

namespace TestMediatR1.Controllers
{
    [Route("ShotgunRoulette/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly JwtService _jwtService;

        public PlayerController(IMediator mediator, JwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        [HttpPost("RegisterPlayer")]
        public async Task<ActionResult<RegisterResponse>> RegisterPlayer([FromBody] RegisterPlayerCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("LoginPlayer")]
        public async Task<ActionResult<LoginResponse>> LoginPlayer([FromBody] LoginPlayerCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}