using SmartInventorySystem.API.Extensions;
using SmartInventorySystem.Application;
using SmartInventorySystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// SQL Server: ConnectionStrings:DefaultConnection in appsettings.json
builder.Services.AddApiServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); // registers ApplicationDbContext

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Inventory System API v1");
    });
}

app.UseHttpsRedirection();

// Custom middleware — register when middleware classes exist
// app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
