using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.ValueObjects;
using FluentValidation;

namespace DeterministicAiLab.Application.Carts.Commands;

public class ApplyPromoCodeCommandValidator : AbstractValidator<ApplyPromoCodeCommand>
{
    public ApplyPromoCodeCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("Cart ID is required.");

        RuleFor(x => x.PromoCodeText)
            .NotEmpty().WithMessage("Promo code text is required.")
            .Must(BeAValidPromoCodeText).WithMessage("Invalid promo code format.");
    }

    private bool BeAValidPromoCodeText(string promoCodeText)
    {
        try
        {
            _ = new PromoCodeText(promoCodeText);
            return true;
        }
        catch
        {
            return false;
        }
    }
}