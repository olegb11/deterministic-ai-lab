using System;
using Xunit;
using FluentAssertions;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Domain.Tests
{
    public class CartTests
    {
        [Fact]
        public void CalculateTotal_WithEmptyCart_ReturnsZero()
        {
            // Arrange
            var cart = new Cart();

            // Act
            var total = cart.CalculateTotal();

            // Assert
            total.Should().Be(0m);
        }

        [Fact]
        public void CalculateTotal_WithItemsAndNoPromo_ReturnsCorrectSum()
        {
            // Arrange
            var cart = new Cart();
            cart.AddItem(new CartItem(100m, 2, 0)); // 200
            cart.AddItem(new CartItem(50m, 1, 10));  // 45 

            // Act
            var total = cart.CalculateTotal();

            // Assert
            total.Should().Be(245m);
        }

        [Fact]
        public void ApplyPromoCode_ValidPromo_AppliesDiscountToTotal()
        {
            // Arrange
            var cart = new Cart();
            cart.AddItem(new CartItem(100m, 1, 0)); // 100
            var promo = new PromoCode("SALE20", 20, DateTimeOffset.UtcNow.AddDays(5));

            // Act
            cart.ApplyPromoCode(promo, DateTimeOffset.UtcNow);
            var total = cart.CalculateTotal();

            // Assert
            cart.AppliedPromoCode.Should().Be(promo);
            total.Should().Be(80m); // 100 со скидкой 20%
        }

        [Fact]
        public void ApplyPromoCode_ExpiredPromo_ThrowsException()
        {
            // Arrange
            var cart = new Cart();
            var expiredPromo = new PromoCode("OLDD10", 10, DateTimeOffset.UtcNow.AddDays(-1));

            // Act
            Action act = () => cart.ApplyPromoCode(expiredPromo, DateTimeOffset.UtcNow);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*expired*");
        }
    }
}