using DeterministicAiLab.Contracts.Carts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace DeterministicAiLab.Api.Controllers;

[ApiController]
[Route("api/carts")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly ISender _sender;

    public CartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{cartId}/items")]
    public async Task<IActionResult> AddItem(
        Guid cartId,
        [FromBody] AddItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(
            cartId,
            request.ProductId,
            request.Quantity,
            request.UnitPrice
        );

        await _sender.Send(command, cancellationToken);

        return Ok();
    }
}

public record AddItemRequest(string ProductId, int Quantity, decimal UnitPrice);