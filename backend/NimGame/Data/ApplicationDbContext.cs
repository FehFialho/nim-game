using Microsoft.EntityFrameworkCore;
using NimGame.Models;

namespace NimGame.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        // Se quiser, deixe aqui outros DbSet, por exemplo:
        // public DbSet<Game> Games { get; set; }
    }
}
