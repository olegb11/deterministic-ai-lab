namespace DeterministicAiLab.Domain.Entities;

using ValueObjects;

public class Cart
{
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public PromoCode? AppliedPromoCode { get; private set; }

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public void ApplyPromoCode(PromoCode promoCode, DateTimeOffset currentDate)
    {
        if (!promoCode.IsValidAt(currentDate))
            throw new InvalidOperationException("Cannot apply invalid or expired promo code");

        AppliedPromoCode = promoCode;
    }

    public decimal CalculateTotal()
    {
        if (_items.Count == 0)
            return 0.0m;

        decimal subtotal = _items.Sum(i => i.Total);

        if (AppliedPromoCode != null)
        {
            decimal discountFactor = (100 - AppliedPromoCode.DiscountPercentage) / 100m;
            subtotal *= discountFactor;
        }

        return Math.Round(subtotal, 2);
    }
}