using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeterministicAiLab.Application.Carts;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Infrastructure.Persistence.Carts;

/// <summary>
/// PostgreSQL implementation of IPromoCodeRepository using Dapper.
/// </summary>
public class SqlPromoCodeRepository : IPromoCodeRepository
{
    private readonly IDbConnection _connection;

    public SqlPromoCodeRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<PromoCode?> GetByCodeAsync(PromoCodeText codeText, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT code, discount_percentage AS DiscountPercentage, expiration_date AS ExpirationDate, is_active AS IsActive
            FROM promo_codes
            WHERE code = @Code;
        ";

        var command = new CommandDefinition(sql, new { Code = codeText.Value }, cancellationToken: cancellationToken);

        var dto = await _connection.QuerySingleOrDefaultAsync<PromoCodeDto>(command);

        if (dto == null)
        {
            return null;
        }

        return new PromoCode(
            new PromoCodeText(dto.Code),
            dto.DiscountPercentage,
            dto.ExpirationDate,
            dto.IsActive
        );
    }

    private sealed record PromoCodeDto(
        string Code,
        decimal DiscountPercentage,
        System.DateTime ExpirationDate,
        bool IsActive
    );
}