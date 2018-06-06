using Microsoft.EntityFrameworkCore;
using Service_Repository_Layer.Entity;

namespace Service_Repository_Layer.Repo
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            
        }
        public DbSet<User> User { get; set; }
        
        public DbSet<Log> Log { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Log>()
                .HasKey(e => e.Id);           
            base.OnModelCreating(modelBuilder);
        }
    }
}
