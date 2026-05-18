using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Constants;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.ApplicationUsers.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = new[]
        {
            new ApplicationUser(
                "admin",
                _passwordHasher.HashPassword("Admin@123"),
                Roles.Admin),
            new ApplicationUser(
                "employee",
                _passwordHasher.HashPassword("Employee@123"),
                Roles.Employee)
        };

        await _context.ApplicationUsers.AddRangeAsync(users, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
