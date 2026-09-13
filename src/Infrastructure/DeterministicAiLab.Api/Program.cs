using DeterministicAiLab.Api.Extensions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure, application, and authentication services
builder.Services
    .AddInfrastructure()
    .AddApplicationServices()
    .AddAuthenticationAndAuthorization()
    .AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCustomSwagger();
app.UseCustomPipeline();

app.MapEndpoints();

app.Run();

