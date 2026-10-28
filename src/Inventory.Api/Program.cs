using Inventory.Api.Repositories;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("Inventory")
        ?? throw new InvalidOperationException("Set ConnectionStrings__Inventory before starting the API.");
    return NpgsqlDataSource.Create(connectionString);
});
builder.Services.AddScoped<InventoryRepository>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();
app.Run();
