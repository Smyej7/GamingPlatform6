using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Clé étrangère vers l'utilisateur
        public User User { get; set; } // Navigation vers le modèle User

        [Required]
        public int GameId { get; set; } // Clé étrangère vers le jeu
        public Game Game { get; set; } // Navigation vers le modèle Game

        [Required]
        public int Points { get; set; } // Le score de l'utilisateur dans ce jeu

        public DateTime AchievedAt { get; set; } = DateTime.Now; // Date à laquelle le score a été obtenu
    }
}
