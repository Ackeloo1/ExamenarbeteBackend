using MediatR;
using Microsoft.AspNetCore.Mvc;
using TestMediatR1.Game.Models;
using TestMediatR1.Player.Commands;
using TestMediatR1.Player.Queries;
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

        [HttpPost("AddScore")]
        public async Task<ActionResult<ScoreResponse>> AddScore([FromBody] AddScoreCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("GetScore")]
        public async Task<ActionResult<ScoreResponse>> GetScore([FromQuery] string player1Name, [FromQuery] string player2Name)
        {
            var query = new GetScoreQuery { Player1Name = player1Name, Player2Name = player2Name };
            var response = await _mediator.Send(query);
            return Ok(response);
        }

    }
}