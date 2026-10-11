using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Infrastructure.Persistence;
using DeterministicAiLab.Infrastructure.Persistence.Carts;
using DeterministicAiLab.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // 1. Register connection factory as Singleton
        services.AddSingleton<IDbConnectionFactory>(_ =>
            new PostgresConnectionFactory(connectionString));

        // 2. Register IDbConnection scoped per HTTP request (required by SqlPromoCodeRepository)
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

        // 3. Register repositories with Scoped lifetime
        services.AddScoped<ICartRepository, SqlCartRepository>();
        services.AddScoped<IPromoCodeRepository, SqlPromoCodeRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(Application.Carts.Commands.AddItemToCartCommandHandler).Assembly;

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddOpenBehavior(typeof(Application.Behaviors.ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("Jwt:Secret not found.");
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