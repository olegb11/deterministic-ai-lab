using DeterministicAiLab.Domain.Entities;

namespace DeterministicAiLab.Application.Carts;

/// <summary>
/// Repository port for managing Shopping Cart aggregate persistence.
/// Implemented in the Infrastructure layer using Dapper and PostgreSQL.
/// </summary>
public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task SaveAsync(Cart cart, CancellationToken cancellationToken = default);
}