using System;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Domain.Tests.ValueObjects;

public class PromoCodeTextTests
{
    [Theory]
    [InlineData("SAVE10")]
    [InlineData("promo1")] // Should be normalized to uppercase
    public void Constructor_WithValidCode_NormalizesToUppercaseAndSucceeds(string input)
    {
        // Act
        var promoCode = new PromoCodeText(input);

        // Assert
        promoCode.Value.Should().Be(input.Trim().ToUpperInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TOOLONG")] // 7 characters
    [InlineData("SHRT")]    // 4 characters
    [InlineData("SAV-10")]  // Special character
    public void Constructor_WithInvalidFormat_ThrowsArgumentException(string input)
    {
        // Arrange & Act
        Action act = () => new PromoCodeText(input);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}