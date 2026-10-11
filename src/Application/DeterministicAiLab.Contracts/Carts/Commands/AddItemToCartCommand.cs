using MediatR;

namespace DeterministicAiLab.Contracts.Carts.Commands;

/// <summary>
/// Contract representing the command to add an item to a shopping cart.
/// </summary>
public record AddItemToCartCommand(
    Guid CartId,
    string ProductId,
    int Quantity,
    decimal UnitPrice
) : IRequest;