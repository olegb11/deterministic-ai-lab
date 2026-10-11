using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace DeterministicAiLab.Api.IntegrationTests;

public class CartsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CartsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddItemToCart_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var cartId = Guid.NewGuid();
        var command = new
        {
            ProductId = "prod-123",
            Quantity = 2,
            UnitPrice = 100.00m
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/carts/{cartId}/items", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddItemToCart_WithValidToken_ReturnsSuccessOrBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateJwtToken();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var cartId = Guid.NewGuid();
        var command = new
        {
            ProductId = "prod-123",
            Quantity = 2,
            UnitPrice = 100.00m
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/carts/{cartId}/items", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ApplyPromoCode_WithValidToken_ReturnsSuccessOrBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateJwtToken();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var cartId = Guid.NewGuid();
        var command = new
        {
            PromoCodeText = "SAVE10"
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/carts/{cartId}/promo", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ApplyPromoCode_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateJwtToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var cartId = Guid.NewGuid();
        var command = new
        {
            PromoCodeText = "TOOLONGCODE" // Violates the ^[A-Z0-9]{6}$ pattern
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/carts/{cartId}/promo", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddItemToCart_WithInvalidQuantity_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateJwtToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var cartId = Guid.NewGuid();
        var command = new
        {
            ProductId = "prod-123",
            Quantity = 0, // Invalid quantity
            UnitPrice = 100.00m
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/carts/{cartId}/items", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    private static string GenerateJwtToken()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var secretKey = configuration["Jwt:Secret"]
                        ?? throw new InvalidOperationException("Jwt:Secret is missing in configuration.");
        var issuer = configuration["Jwt:Issuer"] 
                        ?? throw new InvalidOperationException("Jwt:Issuer is missing in configuration.");
        var audience = configuration["Jwt:Audience"] 
                        ?? throw new InvalidOperationException("Jwt:Audience is missing in configuration.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.Role, "Customer")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}