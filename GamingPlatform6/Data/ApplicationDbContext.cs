using GamingPlatform6.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingPlatform6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurer Username comme clé primaire pour User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserName); // Définir Username comme clé primaire

            // Configurer la relation entre Score et User (Pas de suppression en cascade)
            modelBuilder.Entity<Score>()
                .HasOne<User>() // Un Score est lié à un User
                .WithMany()  // Pas de navigation inverse dans User
                .HasForeignKey(s => s.UserId)  // Utilisation de UserId comme clé étrangère
                .OnDelete(DeleteBehavior.Restrict); // Pas de suppression en cascade de Score lorsqu'un User est supprimé

            // Configurer la relation entre Score et Game (Pas de suppression en cascade)
            modelBuilder.Entity<Score>()
                .HasOne<Game>()  // Un Score est lié à un Game
                .WithMany()  // Pas de navigation inverse dans Game
                .HasForeignKey(s => s.GameId)  // Utilisation de GameId comme clé étrangère
                .OnDelete(DeleteBehavior.Restrict); // Pas de suppression en cascade de Score lorsqu'un Game est supprimé

            base.OnModelCreating(modelBuilder);
        }
    }
}
