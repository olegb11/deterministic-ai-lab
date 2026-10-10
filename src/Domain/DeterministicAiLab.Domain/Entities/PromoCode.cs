using System;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Domain.Entities;

/// <summary>
/// Represents a discount promo code entity applied to shopping carts.
/// Invariant PC1: Discount percentage must be between 0 and 100.
/// </summary>
public class PromoCode
{
    public PromoCodeText Code { get; }
    public decimal DiscountPercentage { get; }
    public DateTime ExpirationDate { get; }
    public bool IsActive { get; }

    public PromoCode(PromoCodeText code, decimal discountPercentage, DateTime expirationDate, bool isActive)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100.", nameof(discountPercentage));
        }

        Code = code;
        DiscountPercentage = discountPercentage;
        ExpirationDate = expirationDate;
        IsActive = isActive;
    }

    public bool IsValid(DateTime currentUtcDateTime)
    {
        return IsActive && currentUtcDateTime <= ExpirationDate;
    }
}