using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string GameId { get; set; }

        [Required]
        public int Points { get; set; }

        public DateTime AchievedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"Score: {Points}, GameId: {GameId}, UserId: {UserId}, AchievedAt: {AchievedAt}, Id: {Id}";
        }
    }
}
