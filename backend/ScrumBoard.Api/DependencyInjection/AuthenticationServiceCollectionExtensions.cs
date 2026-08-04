using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using ScrumBoard.Infrastructure.Security;

namespace ScrumBoard.Api.DependencyInjection;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddApiAuthenticationAndControllers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        var jwtSettings = configuration.GetSection("Jwt:JwtSettings").Get<JwtSettings>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accesToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accesToken) &&
                        context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accesToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}