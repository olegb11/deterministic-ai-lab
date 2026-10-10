using System;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Domain.Tests.Entities;

public class PromoCodeTests
{
    [Fact]
    public void IsValid_WithActiveAndNotExpiredCode_ReturnsTrue()
    {
        // Arrange
        var codeText = new PromoCodeText("SAVE10");
        var promo = new PromoCode(codeText, 10.00m, DateTime.UtcNow.AddDays(1), true);

        // Act
        var isValid = promo.IsValid(DateTime.UtcNow);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithExpiredCode_ReturnsFalse()
    {
        // Arrange
        var codeText = new PromoCodeText("SAVE10");
        var promo = new PromoCode(codeText, 10.00m, DateTime.UtcNow.AddDays(-1), true);

        // Act
        var isValid = promo.IsValid(DateTime.UtcNow);

        // Assert
        isValid.Should().BeFalse();
    }
}