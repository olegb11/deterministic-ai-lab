using System.Data;
using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Infrastructure.Persistence.Carts;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DeterministicAiLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Register IDbConnection scoped per HTTP request
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

        // Register feature repositories
        services.AddScoped<ICartRepository, SqlCartRepository>();
        services.AddScoped<IPromoCodeRepository, SqlPromoCodeRepository>();

        return services;
    }
}