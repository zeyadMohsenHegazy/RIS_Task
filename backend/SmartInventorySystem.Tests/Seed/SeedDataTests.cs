using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SmartInventorySystem.Domain.Constants;
using SmartInventorySystem.Infrastructure.Authentication;
using SmartInventorySystem.Infrastructure.Persistence;
using SmartInventorySystem.Infrastructure.Seed;

namespace SmartInventorySystem.Tests.Seed;

public class SeedDataTests
{
    [Fact]
    public void EnsureValid_WithCurrentSeedData_DoesNotThrow()
    {
        var action = () => SeedData.EnsureValid();
        action.Should().NotThrow();
    }

    [Fact]
    public async Task DatabaseSeeder_WithFreshDatabase_SeedsExpectedCounts()
    {
        await using var context = CreateContext();
        var passwordHasher = new PasswordHasher();
        var seeder = new DatabaseSeeder(context, passwordHasher, NullLogger<DatabaseSeeder>.Instance);

        await seeder.SeedAsync();

        (await context.ApplicationUsers.CountAsync()).Should().Be(2);
        (await context.Warehouses.CountAsync()).Should().Be(SeedData.Warehouses.Count);
        (await context.Products.CountAsync()).Should().Be(SeedData.Products.Count);
        (await context.InventoryTransactions.CountAsync()).Should().Be(SeedData.InventoryTransactions.Count);

        var admin = await context.ApplicationUsers.SingleAsync(u => u.Username == SeedData.AdminUsername);
        admin.Role.Should().Be(Roles.Admin);
        passwordHasher.VerifyPassword(SeedData.AdminPassword, admin.PasswordHash).Should().BeTrue();

        var employee = await context.ApplicationUsers.SingleAsync(u => u.Username == SeedData.EmployeeUsername);
        employee.Role.Should().Be(Roles.Employee);
        passwordHasher.VerifyPassword(SeedData.EmployeePassword, employee.PasswordHash).Should().BeTrue();

        var transactions = await context.InventoryTransactions
            .Include(t => t.CreatedByUser)
            .Include(t => t.Product)
            .ToListAsync();

        transactions.Should().OnlyContain(t =>
            t.CreatedByUserId == admin.Id || t.CreatedByUserId == employee.Id);
        transactions.Should().OnlyContain(t => t.ProductId > 0);
    }

    [Fact]
    public async Task DatabaseSeeder_WhenAlreadySeeded_IsIdempotent()
    {
        await using var context = CreateContext();
        var passwordHasher = new PasswordHasher();
        var seeder = new DatabaseSeeder(context, passwordHasher, NullLogger<DatabaseSeeder>.Instance);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        (await context.Products.CountAsync()).Should().Be(SeedData.Products.Count);
        (await context.InventoryTransactions.CountAsync()).Should().Be(SeedData.InventoryTransactions.Count);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
