using System;
using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class ActionLog
    {
        [Key]
        public int Id { get; set; } // Identifiant unique pour chaque log

        [Required]
        public string GameId { get; set; } // Identifiant du jeu associé

        [Required]
        public string UserId { get; set; } // Identifiant de l'utilisateur ayant effectué l'action

        [Required]
        public int Partie { get; set; } // Numéro de la partie pour différencier les sessions de jeu

        [Required]
        [StringLength(100)]
        public string Action { get; set; } // Nom ou type de l'action (par exemple, "Move", "Attack")

        [StringLength(500)]
        public string Description { get; set; } // Description détaillée de l'action (facultative)

        public DateTime LoggedAt { get; set; } = DateTime.Now; // Date et heure de l'action pour le suivi temporel
    }
}
