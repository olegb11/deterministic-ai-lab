using Dapper;
using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using DeterministicAiLab.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeterministicAiLab.Infrastructure.Repositories;

public class SqlCartRepository : ICartRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlCartRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Flat multi-query package with explicit column aliases for proper Dapper mapping matching schema.sql
        const string sql = @"
            -- 1. Check cart existence
            SELECT cart_id FROM carts WHERE cart_id = @CartId;
            
            -- 2. Get cart items with explicit aliases for Dapper property matching
            SELECT product_id AS ProductId, quantity AS Quantity, unit_price AS UnitPrice 
            FROM cart_items 
            WHERE cart_id = @CartId;

            -- 3. Get applied promo code details with explicit aliases for Dapper property matching
            SELECT p.code AS Code, 
                   p.discount_percentage AS DiscountPercentage, 
                   p.expiration_date AS ExpirationDate, 
                   p.is_active AS IsActive
            FROM carts c
            INNER JOIN promo_codes p ON c.applied_promo_code = p.code
            WHERE c.cart_id = @CartId;";

        var command = new CommandDefinition(sql, new { CartId = cartId }, cancellationToken: cancellationToken);
        using var multi = await connection.QueryMultipleAsync(command);

        var existingCartId = await multi.ReadSingleOrDefaultAsync<Guid?>();
        if (!existingCartId.HasValue)
        {
            return null;
        }

        var itemDtos = await multi.ReadAsync<CartItemDto>();
        var items = itemDtos.Select(dto => new CartItem(
            dto.ProductId,
            dto.Quantity,
            new Money(dto.UnitPrice)
        ));

        var promoCodeDto = await multi.ReadSingleOrDefaultAsync<PromoCodeDto>();
        PromoCode? promoCode = null;
        if (promoCodeDto != null)
        {
            promoCode = new PromoCode(
                new PromoCodeText(promoCodeDto.Code),
                promoCodeDto.DiscountPercentage,
                promoCodeDto.ExpirationDate,
                promoCodeDto.IsActive
            );
        }

        return Cart.Load(cartId, items, promoCode);
    }

    public async Task SaveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Upsert cart header incorporating applied_promo_code and mandatory updated_at
            const string upsertCartSql = @"
                INSERT INTO carts (cart_id, applied_promo_code, updated_at)
                VALUES (@CartId, @AppliedPromoCode, @UpdatedAt)
                ON CONFLICT (cart_id) 
                DO UPDATE SET applied_promo_code = EXCLUDED.applied_promo_code, updated_at = EXCLUDED.updated_at;";

            var upsertCommand = new CommandDefinition(
                upsertCartSql,
                new
                {
                    CartId = cart.CartId,
                    AppliedPromoCode = cart.AppliedPromoCode?.Code.Value,
                    UpdatedAt = DateTime.UtcNow
                },
                transaction: transaction,
                cancellationToken: cancellationToken
            );
            await connection.ExecuteAsync(upsertCommand);

            // 2. Synchronize cart items (clear and re-insert strategy within transaction)
            const string deleteItemsSql = "DELETE FROM cart_items WHERE cart_id = @CartId;";
            var deleteCommand = new CommandDefinition(
                deleteItemsSql,
                new { CartId = cart.CartId },
                transaction: transaction,
                cancellationToken: cancellationToken
            );
            await connection.ExecuteAsync(deleteCommand);

            const string insertItemSql = @"
                INSERT INTO cart_items (cart_id, product_id, quantity, unit_price) 
                VALUES (@CartId, @ProductId, @Quantity, @UnitPrice);";

            foreach (var item in cart.Items)
            {
                var insertCommand = new CommandDefinition(
                    insertItemSql,
                    new
                    {
                        CartId = cart.CartId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice.Amount
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken
                );
                await connection.ExecuteAsync(insertCommand);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #region Internal DTOs for Database Mapping
    private class CartItemDto
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Matches unit_price column in schema.sql
    }

    private class PromoCodeDto
    {
        public string Code { get; set; } = default!;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
    #endregion
}