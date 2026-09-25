using System;

namespace DeterministicAiLab.Domain.Entities
{
    public class PromoCode
    {
        public string Code { get; }
        public int DiscountPercentage { get; }
        public DateTimeOffset ExpirationDate { get; }
        public DateTimeOffset CreatedAt { get; }

        public PromoCode(string code, int discountPercentage, DateTimeOffset expirationDate, DateTimeOffset createdAt)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Promo code cannot be empty", nameof(code));
            if (code.Trim().Length != 6) throw new ArgumentException("Promo code must be 6 characters", nameof(code));
            if (discountPercentage < 1 || discountPercentage > 100) throw new ArgumentOutOfRangeException(nameof(discountPercentage), "Discount percentage must be between 1 and 100");

            Code = code;
            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            CreatedAt = createdAt;
        }

        public bool IsValid(DateTimeOffset now)
        {
            return ExpirationDate >= now;
        }
    }
}
