namespace DeterministicAiLab.Domain.Entities;

public class CartItem
{
    public decimal Price { get; }
    public int Quantity { get; }
    public decimal Discount { get; }

    public decimal Total => Math.Round(Price * Quantity * (1 - Discount / 100m), 2);

    public CartItem(decimal price, int quantity, decimal discount)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");
        if (quantity < 1)
            throw new ArgumentException("Quantity must be at least 1");
        if (discount < 0)
            throw new ArgumentException("Discount cannot be negative");
        if (discount > 100)
            throw new ArgumentException("Discount cannot exceed 100%");

        Price = price;
        Quantity = quantity;
        Discount = discount;
    }
}