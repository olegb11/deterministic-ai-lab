using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Application.Carts.Commands;
using DeterministicAiLab.Contracts.Carts.Commands;
using DeterministicAiLab.Domain.Entities;
using NSubstitute;

namespace DeterministicAiLab.Application.Tests.Carts.Commands;

public class AddItemToCartCommandHandlerTests
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly AddItemToCartCommandHandler _handler;

    public AddItemToCartCommandHandlerTests()
    {
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _handler = new AddItemToCartCommandHandler(_cartRepositoryMock);
    }

    [Fact]
    public async Task HandleAsync_WhenCartDoesNotExist_CreatesNewCartAndAddsItem()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var command = new AddItemToCartCommand(cartId, "PROD-123", 2, 150.00m);

        // Configure mock to return null when cart is not found
        _cartRepositoryMock
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartRepositoryMock.Received(1).GetByIdAsync(Arg.Is(cartId), Arg.Any<CancellationToken>());

        await _cartRepositoryMock.Received(1).SaveAsync(
            Arg.Is<Cart>(c =>
                c.CartId == cartId &&
                c.Items.Count == 1 &&
                c.Subtotal.Amount == 300.00m
            ),
            Arg.Any<CancellationToken>()
        );
    }
}