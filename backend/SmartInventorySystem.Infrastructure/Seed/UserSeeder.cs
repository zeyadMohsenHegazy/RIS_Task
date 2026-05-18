using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Constants;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

internal static class UserSeeder
{
    public static async Task<Dictionary<string, ApplicationUser>> SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        if (await context.ApplicationUsers.AnyAsync(cancellationToken))
        {
            var existing = await context.ApplicationUsers
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return existing.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);
        }

        var users = new Dictionary<string, ApplicationUser>(StringComparer.OrdinalIgnoreCase)
        {
            [SeedData.AdminUsername] = new ApplicationUser(
                SeedData.AdminUsername,
                passwordHasher.HashPassword(SeedData.AdminPassword),
                Roles.Admin),
            [SeedData.EmployeeUsername] = new ApplicationUser(
                SeedData.EmployeeUsername,
                passwordHasher.HashPassword(SeedData.EmployeePassword),
                Roles.Employee)
        };

        await context.ApplicationUsers.AddRangeAsync(users.Values, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return users;
    }
}
