using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using DeterministicAiLab.Infrastructure.Persistence;
using DeterministicAiLab.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace DeterministicAiLab.Infrastructure.Tests;

public class SqlCartRepositoryInfrastructureTests
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly SqlCartRepository _repository;

    public SqlCartRepositoryInfrastructureTests()
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing in configuration.");

        _connectionFactory = new PostgresConnectionFactory(connectionString);
        _repository = new SqlCartRepository(_connectionFactory);

        // Ensure database schema is created from schema.sql on test initialization
        InitializeDatabaseSchema();
    }

    private void InitializeDatabaseSchema()
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "schema.sql");

        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Could not find database schema file at: {schemaPath}");
        }

        var schemaSql = File.ReadAllText(schemaPath);

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();
    }

    [Fact]
    public async Task SaveAsync_And_GetByIdAsync_ShouldRoundTripCartSuccessfully()
    {
        // Arrange
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        // Clean up previous test data to ensure test isolation
        using var cleanupCmd = connection.CreateCommand();
        cleanupCmd.CommandText = "DELETE FROM cart_items; DELETE FROM carts; DELETE FROM promo_codes;";
        cleanupCmd.ExecuteNonQuery();

        // Insert a valid promo code first to satisfy foreign key constraints (using SAVE10)
        const string insertPromoSql = @"
            INSERT INTO promo_codes (code, discount_percentage, expiration_date, is_active)
            VALUES ('SAVE10', 10.00, @ExpirationDate, true);";

        using var promoCmd = connection.CreateCommand();
        promoCmd.CommandText = insertPromoSql;
        var param = promoCmd.CreateParameter();
        param.ParameterName = "@ExpirationDate";
        param.Value = DateTime.UtcNow.AddDays(30);
        promoCmd.Parameters.Add(param);
        promoCmd.ExecuteNonQuery();

        var cartId = Guid.NewGuid();

        // Instantiate new cart and add item matching CartItem signature
        var cart = new Cart(cartId);
        cart.AddItem(new CartItem("prod-1", 2, new Money(50.00m)));

        var promoCode = new PromoCode(
            new PromoCodeText("SAVE10"),
            10.00m,
            DateTime.UtcNow.AddDays(30),
            true
        );
        cart.ApplyPromoCode(promoCode);

        // Act
        await _repository.SaveAsync(cart);
        var loadedCart = await _repository.GetByIdAsync(cartId);

        // Assert
        Assert.NotNull(loadedCart);
        Assert.Equal(cart.CartId, loadedCart.CartId);
        Assert.Single(loadedCart.Items);

        var item = loadedCart.Items.First();
        Assert.Equal("prod-1", item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(50.00m, item.UnitPrice.Amount);

        Assert.NotNull(loadedCart.AppliedPromoCode);
        Assert.Equal("SAVE10", loadedCart.AppliedPromoCode.Code.Value);
        Assert.Equal(10.00m, loadedCart.AppliedPromoCode.DiscountPercentage);
    }
}