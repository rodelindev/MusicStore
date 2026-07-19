using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        /*modelBuilder.Entity<Genre>(x => x.ToTable("genres"));
        modelBuilder.Entity<Genre>(x => x.ToTable("genres"));
        modelBuilder.Entity<Genre>(x => x.ToTable("genres"));
        modelBuilder.Entity<Genre>(x => x.ToTable("genres"));
        modelBuilder.Entity<Genre>(x => x.ToTable("genres"));
        modelBuilder.Entity<Genre>(x => x.ToTable("genres"));*/
    }
    //public DbSet<Genre> Genres { get; set; }
}
