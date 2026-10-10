using System;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new Money(-1.00m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Money amount cannot be negative*");
    }

    [Theory]
    [InlineData(10.555, 10.56)] // Invariant M2: MidpointRounding.AwayFromZero check
    [InlineData(10.554, 10.55)]
    [InlineData(0.005, 0.01)]
    public void Constructor_AppliesAwayFromZeroRounding(decimal input, decimal expected)
    {
        // Act
        var money = new Money(input);

        // Assert
        money.Amount.Should().Be(expected);
    }

    [Fact]
    public void Addition_Operator_ReturnsCorrectSum()
    {
        // Arrange
        var m1 = new Money(100.50m);
        var m2 = new Money(50.25m);

        // Act
        var result = m1 + m2;

        // Assert
        result.Amount.Should().Be(150.75m);
    }
}