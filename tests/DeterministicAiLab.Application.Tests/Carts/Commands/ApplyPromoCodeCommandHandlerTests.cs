using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Application.Carts.Commands;
using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace DeterministicAiLab.Application.Tests.Carts.Commands;

public class ApplyPromoCodeCommandHandlerTests
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IPromoCodeRepository _promoCodeRepositoryMock;
    private readonly ApplyPromoCodeCommandHandler _handler;

    public ApplyPromoCodeCommandHandlerTests()
    {
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _promoCodeRepositoryMock = Substitute.For<IPromoCodeRepository>();
        _handler = new ApplyPromoCodeCommandHandler(_cartRepositoryMock, _promoCodeRepositoryMock);
    }

    [Fact]
    public async Task HandleAsync_WhenCartAndPromoCodeExist_AppliesPromoCodeAndSavesCart()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = new Cart(cartId);
        cart.AddItem(new CartItem("PROD-1", 1, new Money(200.00m)));

        var command = new ApplyPromoCodeCommand(cartId, "SAVE10");
        var promoCodeText = new PromoCodeText("SAVE10");
        var promoCode = new PromoCode(promoCodeText, 10.00m, DateTime.UtcNow.AddDays(1), true);

        _cartRepositoryMock
            .GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        _promoCodeRepositoryMock
            .GetByCodeAsync(Arg.Is<PromoCodeText>(t => t.Value == "SAVE10"), Arg.Any<CancellationToken>())
            .Returns(promoCode);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        cart.AppliedPromoCode.Should().NotBeNull();
        var appliedPromoCode = cart.AppliedPromoCode;
        appliedPromoCode.Code.Value.Should().Be("SAVE10");

        await _cartRepositoryMock.Received(1).SaveAsync(cart, Arg.Any<CancellationToken>());
    }
}