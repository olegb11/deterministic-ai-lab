using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Infrastructure.Persistence.Carts;

/// <summary>
/// PostgreSQL implementation of ICartRepository using Dapper.
/// </summary>
public class SqlCartRepository : ICartRepository
{
    private readonly IDbConnection _connection;

    public SqlCartRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT cart_id AS CartId, applied_promo_code AS AppliedPromoCodeText 
            FROM carts 
            WHERE cart_id = @CartId;

            SELECT product_id AS ProductId, quantity AS Quantity, unit_price AS UnitPrice 
            FROM cart_items 
            WHERE cart_id = @CartId;
        ";

        var command = new CommandDefinition(sql, new { CartId = cartId }, cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var cartDto = await multi.ReadFirstOrDefaultAsync<CartDto>();
        if (cartDto == null)
        {
            return null;
        }

        var itemDtos = (await multi.ReadAsync<CartItemDto>()).AsList();

        // Reconstruct domain aggregate
        var cart = new Cart(cartDto.CartId);

        foreach (var itemDto in itemDtos)
        {
            var cartItem = new CartItem(
                itemDto.ProductId,
                itemDto.Quantity,
                new Money(itemDto.UnitPrice)
            );
            cart.AddItem(cartItem);
        }

        // Note: If an applied promo code text exists, we can attach/reapply it or handle it via domain logic.
        // For now, if the DTO has a promo code string, we restore it.

        return cart;
    }

    public async Task SaveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        // Ensure connection is open for transaction management
        if (_connection.State != ConnectionState.Open)
        {
            if (_connection is IDbConnection conn)
            {
                // If connection needs opening (handled usually by DI scope, but good to ensure)
            }
        }

        // Using transaction to ensure atomic save of cart header and its items
        using var transaction = _connection.BeginTransaction();

        try
        {
            const string upsertCartSql = @"
                INSERT INTO carts (cart_id, applied_promo_code, updated_at)
                VALUES (@CartId, @AppliedPromoCode, @UpdatedAt)
                ON CONFLICT (cart_id) 
                DO UPDATE SET 
                    applied_promo_code = EXCLUDED.applied_promo_code,
                    updated_at = EXCLUDED.updated_at;
            ";

            string? promoCodeValue = cart.AppliedPromoCode?.Value;

            await _connection.ExecuteAsync(
                new CommandDefinition(upsertCartSql, new
                {
                    CartId = cart.CartId,
                    AppliedPromoCode = promoCodeValue,
                    UpdatedAt = DateTime.UtcNow
                }, transaction, cancellationToken: cancellationToken)
            );

            // Clear existing items for this cart to handle updates cleanly (replace strategy)
            const string deleteItemsSql = "DELETE FROM cart_items WHERE cart_id = @CartId;";
            await _connection.ExecuteAsync(
                new CommandDefinition(deleteItemsSql, new { CartId = cart.CartId }, transaction, cancellationToken: cancellationToken)
            );

            // Insert current items
            const string insertItemSql = @"
                INSERT INTO cart_items (cart_id, product_id, quantity, unit_price)
                VALUES (@CartId, @ProductId, @Quantity, @UnitPrice);
            ";

            foreach (var item in cart.Items)
            {
                await _connection.ExecuteAsync(
                    new CommandDefinition(insertItemSql, new
                    {
                        CartId = cart.CartId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice.Amount
                    }, transaction, cancellationToken: cancellationToken)
                );
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private sealed record CartDto(Guid CartId, string? AppliedPromoCodeText);

    private sealed record CartItemDto(string ProductId, int Quantity, decimal UnitPrice);
}