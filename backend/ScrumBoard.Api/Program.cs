
using ScrumBoard.Api.DependencyInjection;
using ScrumBoard.Application.DependencyInjection;
using ScrumBoard.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Configuración limpia de Swagger con soporte JWT via Extension Method
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApiAuthenticationAndControllers(builder.Configuration);
builder.Services.AddSignalRServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline and database seeding via Extension Method
await app.ConfigurePipelineAsync();
app.Run();
