using DeterministicAiLab.Domain.Entities;
using FluentAssertions;
using Reqnroll;

namespace DeterministicAiLab.Domain.BDD.Tests.Steps;

[Binding]
public sealed class DiscountPolicySteps
{
    private DiscountPolicy? _policy;
    private bool _promoApplied;

    [Given(@"a shopping cart with total item amount of (.*) UAH")]
    public void GivenAShoppingCartWithTotalItemAmountOfUAH(decimal subtotal)
    {
        _policy = new DiscountPolicy(subtotal);
    }

    [When(@"the promo code ""(.*)"" is applied")]
    public void WhenThePromoCodeIsApplied(string promoCode)
    {
        _policy.Should().NotBeNull();
        _promoApplied = _policy!.ApplyPromoCode(promoCode);
    }

    [Then(@"the total cart amount should be (.*) UAH")]
    public void ThenTheTotalCartAmountShouldBeUAH(decimal expectedTotal)
    {
        _policy!.Total.Should().Be(expectedTotal);
    }

    [Then(@"the final total must never be negative")]
    public void ThenTheFinalTotalMustNeverBeNegative()
    {
        _policy!.Total.Should().BeGreaterThanOrEqualTo(0m);
    }
}