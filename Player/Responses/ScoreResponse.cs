using TestMediatR1.Player.Models;

namespace TestMediatR1.Player.Responses
{
    public class ScoreResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public PlayerScore? Player1Score { get; set; }
        public PlayerScore? Player2Score { get; set; }
    }
}
