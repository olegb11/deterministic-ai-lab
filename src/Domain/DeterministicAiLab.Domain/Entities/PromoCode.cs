namespace DeterministicAiLab.Domain.ValueObjects;

public class PromoCode
{
    public string Code { get; }
    public decimal DiscountPercentage { get; }
    public DateTimeOffset ExpirationDate { get; }

    public PromoCode(string code, decimal discountPercentage, DateTimeOffset expirationDate)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Promo code cannot be empty");
        if (code.Length != 6)
            throw new ArgumentException("Promo code must be 6 characters");
        if (discountPercentage < 1 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 1 and 100");

        Code = code;
        DiscountPercentage = discountPercentage;
        ExpirationDate = expirationDate;
    }

    public bool IsValidAt(DateTimeOffset currentDate) => currentDate <= ExpirationDate;
}