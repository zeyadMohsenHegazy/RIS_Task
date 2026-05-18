namespace SmartInventorySystem.Application.Interfaces;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
