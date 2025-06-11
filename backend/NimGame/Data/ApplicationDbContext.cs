using Microsoft.EntityFrameworkCore;
using NimGame.Models;

namespace NimGame.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Move> Moves { get; set; } = null!;
    }
}
