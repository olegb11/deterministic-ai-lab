using System;
using Xunit;
using FluentAssertions;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Domain.Tests
{
    public class PromoCodeTests
    {
        [Theory]
        [InlineData("SAVE10", 10, 7)]
        [InlineData("HALF20", 50, 30)]
        public void PromoCode_ValidParameters_CreatedSuccessfullyAndValid(string code, decimal discount, int daysValid)
        {
            // Arrange
            var expiration = DateTimeOffset.UtcNow.AddDays(daysValid);

            // Act
            var promoCode = new PromoCode(code, discount, expiration);

            // Assert
            promoCode.Code.Should().Be(code);
            promoCode.DiscountPercentage.Should().Be(discount);
            promoCode.ExpirationDate.Should().Be(expiration);
            promoCode.IsValidAt(DateTimeOffset.UtcNow).Should().BeTrue();
        }

        [Theory]
        [InlineData("", 10, "Promo code cannot be empty")]
        [InlineData("   ", 10, "Promo code cannot be empty")]
        [InlineData("SHORT", 10, "Promo code must be 6 characters")]
        [InlineData("TOOLONG1", 10, "Promo code must be 6 characters")]
        public void PromoCode_InvalidCodeFormat_ThrowsArgumentException(string code, decimal discount, string expectedErrorMessage)
        {
            // Arrange & Act
            Action act = () => new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(5));

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage(expectedErrorMessage);
        }

        [Fact]
        public void PromoCode_NullCode_ThrowsArgumentException()
        {
            // Arrange & Act
            Action act = () => new PromoCode(null!, 10, DateTimeOffset.UtcNow.AddDays(5));

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Promo code cannot be empty");
        }

        [Theory]
        [InlineData(0, "Discount percentage must be between 1 and 100")]
        [InlineData(-5, "Discount percentage must be between 1 and 100")]
        [InlineData(101, "Discount percentage must be between 1 and 100")]
        public void PromoCode_InvalidDiscountRange_ThrowsArgumentException(decimal discount, string expectedErrorMessage)
        {
            // Arrange & Act
            Action act = () => new PromoCode("SAVE10", discount, DateTimeOffset.UtcNow.AddDays(5));

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage(expectedErrorMessage);
        }

        [Fact]
        public void PromoCode_ExpiredDate_IsMarkedAsInvalid()
        {
            // Arrange
            var promoCode = new PromoCode("SAVE10", 10, DateTimeOffset.UtcNow.AddDays(-1));

            // Act
            var isValid = promoCode.IsValidAt(DateTimeOffset.UtcNow);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}

