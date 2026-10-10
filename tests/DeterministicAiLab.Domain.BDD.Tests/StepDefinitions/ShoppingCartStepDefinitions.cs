using System;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;
using FluentAssertions;
using Reqnroll;

namespace DeterministicAiLab.Domain.BDD.Tests.StepDefinitions;

[Binding]
public class ShoppingCartStepDefinitions
{
    private Cart _cart = null!;
    private Exception? _caughtException;

    [Given("a shopping cart with items totaling {float} UAH")]
    public void GivenAShoppingCartWithItemsTotalingUah(float totalAmount)
    {
        _cart = new Cart(Guid.NewGuid());
        if (totalAmount > 0)
        {
            _cart.AddItem(new CartItem("TEST-PROD", 1, new Money((decimal)totalAmount)));
        }
    }

    [When("the user applies the promo code {string}")]
    public void WhenTheUserAppliesThePromoCode(string codeText)
    {
        try
        {
            var promoCode = new PromoCode(new PromoCodeText(codeText), 10.00m, DateTime.UtcNow.AddDays(1), true);
            _cart.ApplyPromoCode(promoCode);
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [When("the user attempts to apply an invalid promo code {string}")]
    public void WhenTheUserAttemptsToApplyAnInvalidPromoCode(string codeText)
    {
        try
        {
            var promoCodeText = new PromoCodeText(codeText);
            var promoCode = new PromoCode(promoCodeText, 10.00m, DateTime.UtcNow.AddDays(1), true);
            _cart.ApplyPromoCode(promoCode);
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [Then("the cart subtotal should remain {float} UAH")]
    public void ThenTheCartSubtotalShouldRemainUah(float expectedSubtotal)
    {
        _cart.Subtotal.Amount.Should().Be((decimal)expectedSubtotal);
    }

    [Then("the applied promo code should be {string}")]
    public void ThenTheAppliedPromoCodeShouldBe(string expectedCode)
    {
        _cart.AppliedPromoCode.Should().NotBeNull();
        _cart.AppliedPromoCode!.Value.Value.Should().Be(expectedCode);
    }

    [Then("the cart total price should become {float} UAH")]
    public void ThenTheCartTotalPriceShouldBecomeUah(float expectedTotal)
    {
        _cart.Total.Amount.Should().Be((decimal)expectedTotal);
    }

    [Then("a domain validation exception should be thrown")]
    public void ThenADomainValidationExceptionShouldBeThrown()
    {
        _caughtException.Should().NotBeNull();
    }

    [Then("the cart total price should remain {float} UAH")]
    public void ThenTheCartTotalPriceShouldRemainUah(float expectedTotal)
    {
        _cart.Total.Amount.Should().Be((decimal)expectedTotal);
    }
}