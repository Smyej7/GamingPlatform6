using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class User
    {
        [Key] // Indiquer que Username sera la clé primaire
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } // Le nom d'utilisateur est maintenant la clé primaire

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"Username: {UserName}, CreatedAt: {CreatedAt}";
        }
    }
}
