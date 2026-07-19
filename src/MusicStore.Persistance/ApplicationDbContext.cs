using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;

namespace MusicStore.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    //public DbSet<Genre> Genres { get; set; }
}
