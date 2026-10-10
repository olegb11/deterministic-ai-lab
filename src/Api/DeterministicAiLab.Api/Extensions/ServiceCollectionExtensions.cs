using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Infrastructure.Persistence;
using DeterministicAiLab.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

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
        // TODO: Will be used for registering MediatR and FluentValidation validators
        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        // TODO: Will be used for registering JWT Bearer authentication and policy-based authorization
        return services;
    }
}
