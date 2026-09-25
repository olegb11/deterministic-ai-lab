using System;
using System.Collections.Generic;

namespace DeterministicAiLab.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; }
        public PromoCode? AppliedPromoCode { get; private set; }
        public DateTimeOffset CreatedAt { get; }

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
        private readonly List<CartItem> _items = new();

        public Cart(Guid id, PromoCode? appliedPromoCode, DateTimeOffset createdAt)
        {
            Id = id;
            AppliedPromoCode = appliedPromoCode;
            CreatedAt = createdAt;
        }

        public void ApplyPromoCode(PromoCode? promo)
        {
            if (promo == null)
            {
                AppliedPromoCode = null;
                return;
            }

            if (!promo.IsValid(DateTimeOffset.UtcNow)) throw new ArgumentException("Cannot apply invalid or expired promo code", nameof(promo));
            AppliedPromoCode = promo;
        }

        public decimal Total(PromoCode? promo = null, DateTimeOffset? now = null)
        {
            decimal sum = 0m;
            foreach (var item in _items)
            {
                sum += item.GetTotal();
            }

            var current = now ?? DateTimeOffset.UtcNow;
            if (promo != null && promo.IsValid(current))
            {
                sum = sum * (1m - (promo.DiscountPercentage / 100m));
            }

            return sum;
        }

        public void AddItem(CartItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.CartId != Id) throw new ArgumentException("cart item belongs to a different cart", nameof(item));
            _items.Add(item);
        }
    }
}
