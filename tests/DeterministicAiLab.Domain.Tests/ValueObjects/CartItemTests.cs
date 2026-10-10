using System;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Domain.Tests.ValueObjects;

public class CartItemTests
{
    [Fact]
    public void Constructor_WithZeroOrNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var unitPrice = new Money(100.00m);

        // Act
        Action actZero = () => new CartItem("PROD-1", 0, unitPrice);
        Action actNegative = () => new CartItem("PROD-1", -1, unitPrice);

        // Assert
        actZero.Should().Throw<ArgumentException>();
        actNegative.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TotalPrice_CalculatesCorrectlyBasedOnQuantityAndUnitPrice()
    {
        // Arrange
        var unitPrice = new Money(150.50m);
        var item = new CartItem("PROD-1", 3, unitPrice);

        // Act
        var total = item.Total;

        // Assert
        total.Amount.Should().Be(451.50m);
    }
}