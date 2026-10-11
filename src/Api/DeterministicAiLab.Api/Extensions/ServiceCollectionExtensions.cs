using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Infrastructure.Persistence;
using DeterministicAiLab.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DeterministicAiLab.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Register connection factory as Singleton (thread-safe and lives for the application lifetime)
        services.AddSingleton<IDbConnectionFactory>(_ =>
            new PostgresConnectionFactory(
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

        // 2. Register repositories with Scoped lifetime (standard for database work in web apps)
        services.AddScoped<ICartRepository, SqlCartRepository>(); return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DeterministicAiLab.Application.Carts.Commands.AddItemToCartCommandHandler).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        // Get secret from configuration or use a secure fallback for development
        var jwtSecret = configuration["Jwt:Secret"] ?? "SuperSecretKeyForDeterministicAiLabDevelopmentOnly2026!";
        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "DeterministicAiLab.Api",
                    ValidateAudience = true,
                    ValidAudience = "DeterministicAiLab.Client",
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization();

        return services;
    }
}
