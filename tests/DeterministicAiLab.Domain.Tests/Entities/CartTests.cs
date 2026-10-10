using System;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Domain.Tests.Entities;

public class CartTests
{
    [Fact]
    public void NewCart_HasEmptyItemsAndZeroTotal()
    {
        // Arrange & Act
        var cart = new Cart(Guid.NewGuid());

        // Assert
        cart.Items.Should().BeEmpty();
        cart.Subtotal.Amount.Should().Be(0.00m);
        cart.Total.Amount.Should().Be(0.00m);
        cart.AppliedPromoCode.Should().BeNull();
    }

    [Fact]
    public void AddItem_UpdatesSubtotalAndTotalCorrectly()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());
        var item1 = new CartItem("PROD-1", 2, new Money(100.00m));
        var item2 = new CartItem("PROD-2", 1, new Money(300.00m));

        // Act
        cart.AddItem(item1);
        cart.AddItem(item2);

        // Assert
        cart.Items.Should().HaveCount(2);
        cart.Subtotal.Amount.Should().Be(500.00m);
        cart.Total.Amount.Should().Be(500.00m);
    }

    [Fact]
    public void ApplyPromoCode_WithValidPercentagePromo_AppliesDiscountToTotal()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());
        cart.AddItem(new CartItem("PROD-1", 1, new Money(1000.00m)));

        var promoCode = new PromoCode(new PromoCodeText("SAVE10"), 10.00m, DateTime.UtcNow.AddDays(1), true);

        // Act
        cart.ApplyPromoCode(promoCode);

        // Assert
        cart.AppliedPromoCode.Should().NotBeNull();
        var appliedPromoCode = cart.AppliedPromoCode;
        appliedPromoCode.Code.Should().Be(new PromoCodeText("SAVE10"));
        cart.Subtotal.Amount.Should().Be(1000.00m);
        cart.Total.Amount.Should().Be(900.00m); // 10% discount applied
    }
}