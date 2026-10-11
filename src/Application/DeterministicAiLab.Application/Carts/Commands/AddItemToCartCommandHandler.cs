using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using MediatR;

namespace DeterministicAiLab.Application.Carts.Commands;

/// <summary>
/// Handler for AddItemToCartCommand, orchestrating domain logic and repository persistence.
/// </summary>
public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand>
{
    private readonly ICartRepository _cartRepository;

    public AddItemToCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Load existing cart or create a new aggregate root if it doesn't exist yet
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken)
                   ?? new Cart(request.CartId);

        // 2. Execute domain logic via value objects
        var money = new Money(request.UnitPrice);
        var cartItem = new CartItem(request.ProductId, request.Quantity, money);

        cart.AddItem(cartItem);

        // 3. Persist changes through the repository port
        await _cartRepository.SaveAsync(cart, cancellationToken);
    }
}