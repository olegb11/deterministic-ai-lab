using System;
using System.Text.RegularExpressions;

namespace DeterministicAiLab.Domain.ValueObjects;

/// <summary>
/// Represents a validated promo code text identifier.
/// Invariant P1: Must be exactly 6 uppercase alphanumeric characters matching ^[A-Z0-9]{6}$.
/// </summary>
public readonly record struct PromoCodeText
{
    private static readonly Regex CodeRegex = new(@"^[A-Z0-9]{6}$", RegexOptions.Compiled);

    public string Value { get; }

    public PromoCodeText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Promo code cannot be empty.", nameof(value));
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(normalized))
        {
            throw new ArgumentException($"Invalid promo code format '{value}'. Must be exactly 6 alphanumeric characters.", nameof(value));
        }

        Value = normalized;
    }

    public override string ToString() => Value;
}