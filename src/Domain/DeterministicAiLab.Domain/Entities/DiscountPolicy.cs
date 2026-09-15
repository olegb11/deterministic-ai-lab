namespace DeterministicAiLab.Domain.Entities;

public sealed class DiscountPolicy
{
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal Total => Math.Max(0, Subtotal - DiscountAmount);

    public DiscountPolicy(decimal subtotal)
    {
        if (subtotal < 0)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");

        Subtotal = subtotal;
    }

    public bool ApplyPromoCode(string promoCode)
    {
        if (string.Equals(promoCode, "SUMMER2026", StringComparison.OrdinalIgnoreCase))
        {
            // Boundary condition: strictly greater than 1000 UAH
            if (Subtotal > 1000m)
            {
                DiscountAmount = Subtotal * 0.10m;
                return true;
            }
        }

        return false;
    }
}