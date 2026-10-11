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

        // Middleware to catch validation and domain exceptions and return 400 Bad Request
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (FluentValidation.ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                await context.Response.WriteAsJsonAsync(new { Errors = errors });
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
            }
        });

        return app;
    }
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // TODO: API endpoints will be mapped here
        return app;
    }
}
