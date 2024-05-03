using TestMediatR1.Player.Models;

namespace TestMediatR1.Game
{
    public class GameModel
    {
        public string? GameId { get; set; }
        public List<PlayerProps>? Players { get; set; }
        public BulletsModel Bullets { get; set; }
    }

    public class GameStartResponse
    {
        public List<PlayerProps>? Players { get; set; }
        public string? GameId { get; set; }
        public BulletsModel Bullets { get; set; }

        public static implicit operator GameStartResponse(GameModel model)
        {
            return new GameStartResponse
            {
                Players = model.Players,
                GameId = model.GameId,
                Bullets = model.Bullets
            };
        }
    }

    public class BulletsModel
    {
        public int Bullets { get; set; }
        public int Blanks { get; set; }
    }
}
