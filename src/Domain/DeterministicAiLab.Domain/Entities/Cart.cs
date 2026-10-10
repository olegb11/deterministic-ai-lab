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

    public PromoCode? AppliedPromoCode { get; private set; }

    public Money Subtotal => _items.Count == 0 ? Money.Zero : new Money(_items.Sum(i => i.Total.Amount));

    public Money Total
    {
        get
        {
            var subtotalAmount = Subtotal.Amount;
            var discountPercentage = AppliedPromoCode?.DiscountPercentage ?? 0m;

            if (discountPercentage <= 0)
            {
                return Subtotal;
            }

            var discountAmount = subtotalAmount * (discountPercentage / 100m);
            var totalAmount = subtotalAmount - discountAmount;
            return new Money(totalAmount < 0 ? 0m : totalAmount);
        }
    }

    public Cart(Guid cartId)
    {
        CartId = cartId;
    }

    /// <summary>
    /// Reconstructs an existing cart aggregate from persistence storage 
    /// without triggering domain validation rules.
    /// </summary>
    public static Cart Load(Guid cartId, IEnumerable<CartItem> items, PromoCode? appliedPromoCode)
    {
        var cart = new Cart(cartId);

        foreach (var item in items)
        {
            cart._items.Add(item);
        }

        cart.AppliedPromoCode = appliedPromoCode;

        return cart;
    }

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public void ApplyPromoCode(PromoCode promoCode)
    {
        ArgumentNullException.ThrowIfNull(promoCode);

        if (!promoCode.IsValid(DateTime.UtcNow))
        {
            throw new ArgumentException("Promo code is expired or inactive.", nameof(promoCode));
        }

        AppliedPromoCode = promoCode;
    }
}