using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Persistence.Projections;

namespace MusicStore.Persistence;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<MusicStoreUserIdentity>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder.Entity<ConcertDetailView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });
        
        modelBuilder.Entity<SaleReportView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });
        modelBuilder.Entity<MusicStoreUserIdentity>(x => x.ToTable("Users"));
        modelBuilder.Entity<IdentityRole>(x => x.ToTable("Roles"));
        modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRoles"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(x => x.ToTable("RoleClaims"));
        modelBuilder.Entity<IdentityUserClaim<string>>(x => x.ToTable("UserClaims"));
        modelBuilder.Entity<IdentityUserLogin<string>>(x => x.ToTable("UserLogins"));
        modelBuilder.Entity<IdentityUserToken<string>>(x => x.ToTable("UserTokens"));
    }
}
