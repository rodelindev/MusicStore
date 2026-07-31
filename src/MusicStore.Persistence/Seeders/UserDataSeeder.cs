using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Entities;

namespace MusicStore.Persistence.Seeders;

public class UserDataSeeder(IServiceProvider _serviceProvider)
{
    public async Task SeedAsync()
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<MusicStoreUserIdentity>>();
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var adminRole = new IdentityRole("Administrator");
        var customerRole = new IdentityRole("Customer");

        if (!await roleManager.RoleExistsAsync("Administrator"))
            await roleManager.CreateAsync(adminRole);

        if (!await roleManager.RoleExistsAsync("Customer"))
            await roleManager.CreateAsync(customerRole);

        var adminEmail = "admin@musicstore.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new MusicStoreUserIdentity
            {
                FirstName = "Admin",
                LastName = "User",
                UserName = adminEmail,
                Email = adminEmail,
                Age = 30,
                DocumentType = DocumentTypeEnum.Dni,
                DocumentNumber = "12345678",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123*");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
}