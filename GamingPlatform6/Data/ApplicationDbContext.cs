using GamingPlatform6.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GamingPlatform6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Désactiver la suppression en cascade pour Score -> User
            modelBuilder.Entity<Score>()
                .HasOne(s => s.User)
                .WithMany() // Pas de navigation inverse explicitement configurée
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Pas de suppression en cascade

            // Désactiver la suppression en cascade pour Score -> Game
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Game)
                .WithMany()
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Restrict); // Pas de suppression en cascade
        }

    }
}