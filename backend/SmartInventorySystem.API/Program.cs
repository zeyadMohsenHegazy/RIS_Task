using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.API.Extensions;
using SmartInventorySystem.Application;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Infrastructure;
using SmartInventorySystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

await ApplyMigrationsAsync(app);
await SeedDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Inventory System API v1");
    });
}

app.UseCorsPolicy();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseInitialization");
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();

    try
    {
        await seeder.SeedAsync();
        logger.LogInformation("Database initialization (migrations + seed) finished.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database seeding failed.");
        throw;
    }
}
