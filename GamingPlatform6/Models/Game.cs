using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class Game
    {
        [Key]
        [Required]
        [StringLength(100)]
        public required string GameName { get; set; }

        public override string ToString()
        {
            return $"Game: {GameName}";
        }
        //[Required]
        //public DateTime StartTime { get; set; } = DateTime.Now;

        //public DateTime? EndTime { get; set; }

        //// Clé étrangère vers l'utilisateur qui a démarré la partie
        //public int UserId { get; set; }
        //public User User { get; set; }

        //[Required]
        //public GameStatus Status { get; set; } = GameStatus.EnCours;
    }
}
