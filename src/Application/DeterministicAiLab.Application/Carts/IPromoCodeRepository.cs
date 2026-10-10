using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Application.Carts;

/// <summary>
/// Repository port for retrieving Promo Codes.
/// </summary>
public interface IPromoCodeRepository
{
    Task<PromoCode?> GetByCodeAsync(PromoCodeText codeText, CancellationToken cancellationToken = default);
}