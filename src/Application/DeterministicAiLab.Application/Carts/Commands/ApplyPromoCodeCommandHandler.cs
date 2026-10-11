using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.ValueObjects;
using MediatR;

namespace DeterministicAiLab.Application.Carts.Commands;

/// <summary>
/// Handler for ApplyPromoCodeCommand, orchestrating cart loading, promo code validation lookup, and persistence.
/// </summary>
public class ApplyPromoCodeCommandHandler : IRequestHandler<ApplyPromoCodeCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPromoCodeRepository _promoCodeRepository;

    public ApplyPromoCodeCommandHandler(
        ICartRepository cartRepository,
        IPromoCodeRepository promoCodeRepository)
    {
        _cartRepository = cartRepository;
        _promoCodeRepository = promoCodeRepository;
    }

    public async Task Handle(ApplyPromoCodeCommand request, CancellationToken cancellationToken)
    {
        // 1. Load the existing cart
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken)
                   ?? throw new InvalidOperationException($"Cart with ID '{request.CartId}' was not found.");

        // 2. Retrieve the promo code entity
        var promoCodeText = new PromoCodeText(request.PromoCodeText);
        var promoCode = await _promoCodeRepository.GetByCodeAsync(promoCodeText, cancellationToken)
                        ?? throw new InvalidOperationException($"Promo code '{request.PromoCodeText}' was not found.");

        // 3. Apply promo code via domain logic
        cart.ApplyPromoCode(promoCode);

        // 4. Persist updated cart state
        await _cartRepository.SaveAsync(cart, cancellationToken);
    }
}