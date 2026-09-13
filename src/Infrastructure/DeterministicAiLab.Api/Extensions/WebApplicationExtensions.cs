using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace DeterministicAiLab.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }

    public static WebApplication UseCustomPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        
        // TODO: Добавить app.UseAuthentication() и app.UseAuthorization() при внедрении авторизации.
        
        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // TODO: Здесь будут мапиться наши API эндпоинты
        return app;
    }
}
