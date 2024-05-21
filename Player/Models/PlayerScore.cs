using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestMediatR1.Player.Models
{
    public class PlayerScore
    {
        [Key]
        public int ScoreId { get; set; }
        public int Score { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }

        [JsonIgnore]
        public ICollection<PlayerModel>? Players { get; set; }
    }
}
