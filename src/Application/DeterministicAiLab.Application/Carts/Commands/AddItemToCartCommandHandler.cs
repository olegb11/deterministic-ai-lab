using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Application.Carts.Commands;

/// <summary>
/// Handler for AddItemToCartCommand, orchestrating domain logic and repository persistence.
/// </summary>
public class AddItemToCartCommandHandler
{
    private readonly ICartRepository _cartRepository;

    public AddItemToCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task HandleAsync(AddItemToCartCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Load existing cart or create a new aggregate root if it doesn't exist yet
        var cart = await _cartRepository.GetByIdAsync(command.CartId, cancellationToken)
                   ?? new Cart(command.CartId);

        // 2. Execute domain logic via value objects
        var money = new Money(command.UnitPrice);
        var cartItem = new CartItem(command.ProductId, command.Quantity, money);

        cart.AddItem(cartItem);

        // 3. Persist changes through the repository port
        await _cartRepository.SaveAsync(cart, cancellationToken);
    }
}