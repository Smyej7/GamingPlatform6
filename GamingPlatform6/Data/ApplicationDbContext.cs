using GamingPlatform6.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GamingPlatform6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}