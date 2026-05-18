using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace POS.Infrastructure.Data;

public sealed class POSDbContextFactory : IDesignTimeDbContextFactory<POSDbContext>
{
    public POSDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<POSDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=POSDesktopSystem;Trusted_Connection=True;TrustServerCertificate=True;");
        return new POSDbContext(optionsBuilder.Options);
    }
}
