using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.DependencyInjection;
using ScrumBoard.Infrastructure.DependencyInjection;
using ScrumBoard.Infrastructure.Persistence;
using ScrumBoard.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
await InitializeDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
return;

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ScrumBoardDbContext>();
        var dbSeeder = new DatabaseSeeder(context);
        await dbSeeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        logger.LogError(ex, "Ha Ocurrido un error inicializando la Base de dato.");
        throw;
    }
}
