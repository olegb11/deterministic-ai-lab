using System;

namespace DeterministicAiLab.Domain.Entities
{
    public class Item
    {
        public string Code { get; }
        public string Name { get; }
        public decimal Price { get; }
        public string? Description { get; }
        public bool IsActive { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }

        public Item(string code, string name, decimal price, string? description, bool isActive, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("code must not be empty", nameof(code));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name must not be empty", nameof(name));
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "price must be non-negative");

            Code = code;
            Name = name;
            Price = price;
            Description = description;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
