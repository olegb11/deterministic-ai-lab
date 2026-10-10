using System;

namespace DeterministicAiLab.Domain.ValueObjects;

/// <summary>
/// Represents monetary value with strict invariant protection against negative amounts
/// and standardized rounding to prevent discrepancies with databases and payment gateways.
/// Invariant M1: Amount must be >= 0.00.
/// Invariant M2: Rounding mode must be MidpointRounding.AwayFromZero to ensure deterministic commercial calculations.
/// </summary>
public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Money amount cannot be negative.", nameof(amount));
        }

        // Invariant M2 implementation: AwayFromZero rounding
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static Money Zero => new(0m);

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static Money operator -(Money left, Money right) => new(left.Amount - right.Amount);

    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;

    public override string ToString() => Amount.ToString("0.00");
}