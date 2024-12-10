using System.ComponentModel.DataAnnotations;

namespace GamingPlatform6.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
