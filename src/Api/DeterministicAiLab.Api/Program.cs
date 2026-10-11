using DeterministicAiLab.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure, application, and authentication services
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplicationServices()
    .AddAuthenticationAndAuthorization(builder.Configuration)
    .AddSwaggerDocumentation()
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCustomSwagger();
app.UseCustomPipeline();

app.MapControllers();
app.MapEndpoints();

app.Run();