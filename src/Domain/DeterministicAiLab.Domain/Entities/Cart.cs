using System;
using System.Collections.Generic;
using System.Linq;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Domain.Entities;

/// <summary>
/// Represents the Shopping Cart aggregate root, managing items and promo code applications.
/// </summary>
public class Cart
{
    private readonly List<CartItem> _items = new();

    public Guid CartId { get; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public PromoCodeText? AppliedPromoCode { get; private set; }
    private decimal PromoCodeDiscountPercentage { get; set; }

    public Money Subtotal => _items.Count == 0 ? Money.Zero : new Money(_items.Sum(i => i.Total.Amount));

    public Money Total
    {
        get
        {
            var subtotalAmount = Subtotal.Amount;
            if (PromoCodeDiscountPercentage <= 0)
            {
                return Subtotal;
            }

            var discountAmount = subtotalAmount * (PromoCodeDiscountPercentage / 100m);
            var totalAmount = subtotalAmount - discountAmount;
            return new Money(totalAmount < 0 ? 0m : totalAmount);
        }
    }

    public Cart(Guid cartId)
    {
        CartId = cartId;
    }

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public void ApplyPromoCode(PromoCode promoCode)
    {
        if (!promoCode.IsValid(DateTime.UtcNow))
        {
            throw new ArgumentException("Promo code is expired or inactive.", nameof(promoCode));
        }

        AppliedPromoCode = promoCode.Code;
        PromoCodeDiscountPercentage = promoCode.DiscountPercentage;
    }
}