using System;
using Xunit;
using FluentAssertions;
using DeterministicAiLab.Domain.Entities;

namespace DeterministicAiLab.Domain.Tests
{
    public class CartItemTests
    {
        [Theory]
        [InlineData(100.0, 1, 0, 100.0)]
        [InlineData(100.0, 1, 10, 90.0)]
        [InlineData(50.0, 2, 20, 80.0)]
        [InlineData(33.33, 3, 0, 99.99)]
        public void CartItem_ValidParameters_CalculatesTotalCorrectly(decimal price, int quantity, decimal discount, decimal expectedTotal)
        {
            // Arrange & Act
            var item = new CartItem(price, quantity, discount);

            // Assert
            item.Price.Should().Be(price);
            item.Quantity.Should().Be(quantity);
            item.Discount.Should().Be(discount);
            item.Total.Should().Be(expectedTotal);
        }

        [Theory]
        [InlineData(-10.0, 1, 0, "Price cannot be negative")]
        [InlineData(100.0, 0, 0, "Quantity must be at least 1")]
        [InlineData(100.0, -1, 0, "Quantity must be at least 1")]
        [InlineData(100.0, 1, -5, "Discount cannot be negative")]
        [InlineData(100.0, 1, 105, "Discount cannot exceed 100%")]
        public void CartItem_InvalidParameters_ThrowsArgumentException(decimal price, int quantity, decimal discount, string expectedErrorMessage)
        {
            // Arrange & Act
            Action act = () => new CartItem(price, quantity, discount);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage(expectedErrorMessage);
        }
    }
}
