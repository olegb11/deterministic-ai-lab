using System;

namespace DeterministicAiLab.Domain.ValueObjects;

/// <summary>
/// Represents a line item inside a shopping cart, containing product reference, quantity, and unit price.
/// Invariant CI1: Quantity must be greater than zero.
/// </summary>
public readonly record struct CartItem
{
    public string ProductId { get; }
    public int Quantity { get; }
    public Money UnitPrice { get; }

    /// <summary>
    /// Calculates total price for this cart item line.
    /// </summary>
    public Money Total => new(UnitPrice.Amount * Quantity);

    public CartItem(string productId, int quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}