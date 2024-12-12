using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class Lobby
    {
        [Key]
        public string LobbyId { get; set; } // Identifiant unique pour le lobby

        [Required]
        public string HostUserId { get; set; } // L'utilisateur qui a créé le lobby

        public string GuestUserId { get; set; } = ""; // L'utilisateur invité (peut être null)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Date de création du lobby
    }
}
