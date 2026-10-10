using System;

namespace DeterministicAiLab.Contracts.Carts.Commands;

/// <summary>
/// Contract representing the command to apply a promo code to a shopping cart.
/// </summary>
public record ApplyPromoCodeCommand(
    Guid CartId,
    string PromoCodeText
);