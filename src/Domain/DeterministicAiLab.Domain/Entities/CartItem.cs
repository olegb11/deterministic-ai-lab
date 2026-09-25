using System;

namespace DeterministicAiLab.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; }
        public Guid CartId { get; }
        public string ItemCode { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public decimal Discount { get; }
        public DateTimeOffset CreatedAt { get; }

        public CartItem(Guid id, Guid cartId, string itemCode, decimal price, int quantity, decimal discount, DateTimeOffset createdAt)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id must be provided", nameof(id));
            if (cartId == Guid.Empty) throw new ArgumentException("CartId must be provided", nameof(cartId));
            if (string.IsNullOrWhiteSpace(itemCode)) throw new ArgumentException("Item code must not be empty", nameof(itemCode));
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
            if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1");
            if (discount < 0) throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot be negative");
            if (discount > 100) throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot exceed 100%");

            Id = id;
            CartId = cartId;
            ItemCode = itemCode;
            Price = price;
            Quantity = quantity;
            Discount = discount;
            CreatedAt = createdAt;
        }

        public decimal GetTotal()
        {
            // price * quantity * (1 - discount/100)
            var multiplier = 1m - (Discount / 100m);
            return Price * Quantity * multiplier;
        }
    }
}
