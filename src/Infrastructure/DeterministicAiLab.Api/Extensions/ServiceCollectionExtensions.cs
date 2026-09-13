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
        // TODO: Будет использоваться для регистрации Dapper IDbConnection, IUserContext и др.
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // TODO: Будет использоваться для регистрации MediatR и валидаторов FluentValidation
        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        // TODO: Будет использоваться для регистрации JWT Bearer аутентификации и авторизации на основе политик
        return services;
    }
}
