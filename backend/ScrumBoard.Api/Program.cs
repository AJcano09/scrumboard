
using ScrumBoard.Api.DependencyInjection;
using ScrumBoard.Application.DependencyInjection;
using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.DependencyInjection;
using ScrumBoard.Infrastructure.Persistence;
using ScrumBoard.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Configuración limpia de Swagger con soporte JWT via Extension Method
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApiAuthenticationAndControllers(builder.Configuration);

var app = builder.Build();
await InitializeDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
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
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var dbSeeder = new DatabaseSeeder(context,passwordHasher);
        await dbSeeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        logger.LogError(ex, "Ha Ocurrido un error inicializando la Base de dato.");
        throw;
    }
}
