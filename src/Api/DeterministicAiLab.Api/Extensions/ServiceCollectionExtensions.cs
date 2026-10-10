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

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // TODO: Will be used for registering Dapper IDbConnection, IUserContext, etc.
        return services;
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
