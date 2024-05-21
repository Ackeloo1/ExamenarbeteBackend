using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestMediatR1.Player.Models
{
    public class PlayerModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int ScoreId { get; set; }

        [JsonIgnore]
        public PlayerScore? Score { get; set; }
    }
}
