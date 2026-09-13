using System;
using System.Linq;
using Reqnroll;
using FluentAssertions;
using DeterministicAiLab.Domain.Entities;
using DeterministicAiLab.Domain.ValueObjects;

namespace DeterministicAiLab.Domain.BDD.Tests.Steps
{
    [Binding]
    public class ShoppingCartDomainSteps
    {
        private Cart _cart = null!;
        private PromoCode _promoCode = null!;
        private Exception _caughtException = null!;
        private decimal _calculatedTotal;

        // --- Context: Shopping Cart & Items ---

        [Given("an empty shopping cart")]
        [Given("a shopping cart")]
        public void GivenAnEmptyShoppingCart()
        {
            _cart = new Cart();
        }

        [Given("a shopping cart with the following items:")]
        public void GivenAShoppingCartWithTheFollowingItems(DataTable dataTable)
        {
            _cart = new Cart();

            foreach (var row in dataTable.Rows)
            {
                var price = decimal.Parse(row["price"]);
                var quantity = int.Parse(row["quantity"]);
                var discount = int.Parse(row["discount"]);

                var item = new CartItem(price, quantity, discount);
                _cart.AddItem(item);
            }
        }

        [When("I calculate the total price")]
        public void WhenICalculateTheTotalPrice()
        {
            _calculatedTotal = _cart.CalculateTotal();
        }

        [When("no promo code is applied")]
        public void WhenNoPromoCodeIsApplied()
        {
            _calculatedTotal = _cart.CalculateTotal();
        }

        [Then("the total price should be {decimal}")]
        [Then("the cart total should be {decimal}")]
        public void ThenTheTotalShouldBe(decimal expectedTotal)
        {
            _calculatedTotal.Should().Be(expectedTotal);
        }

        // --- Context: Item Creation & Discount Policy ---

        [When("I try to create an item with price {decimal}, quantity {int}, and discount {int}")]
        public void WhenITryToCreateAnItemWithPriceQuantityAndDiscount(decimal price, int quantity, int discount)
        {
            try
            {
                var item = new CartItem(price, quantity, discount);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [When("I add an item with price {decimal}, quantity {int}, and discount {int}")]
        public void WhenIAddAnItemWithPriceQuantityAndDiscount(decimal price, int quantity, int discount)
        {
            _cart ??= new Cart();
            var item = new CartItem(price, quantity, discount);
            _cart.AddItem(item);
            _calculatedTotal = _cart.CalculateTotal();
        }

        [Then("the calculated item total should be {decimal}")]
        public void ThenTheCalculatedItemTotalShouldBe(decimal expectedTotal)
        {
            _calculatedTotal.Should().Be(expectedTotal);
        }

        // --- Context: Promo Codes Policy & Creation ---

        [When("I create a promo code with code {string} and discount {int}%")]
        public void WhenICreateAPromoCodeWithCodeAndDiscount(string code, int discount)
        {
            try
            {
                _promoCode = new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(10));
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [When("I create a promo code with code {string}, discount {int}%, and expiration date in {int} days")]
        public void WhenICreateAPromoCodeWithCodeDiscountAndExpirationDateInDays(string code, int discount, int days)
        {
            try
            {
                _promoCode = new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(days));
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Given("a promo code {string} with {int}% discount that expired {int} day ago")]
        public void GivenAPromoCodeWithDiscountThatExpiredDayAgo(string code, int discount, int daysAgo)
        {
            _promoCode = new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(-daysAgo));
        }

        [Given("an expired promo code {string} with {int}% discount")]
        public void GivenAnExpiredPromoCodeWithDiscount(string code, int discount)
        {
            _promoCode = new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(-1));
        }

        [Given("a valid promo code {string} with {int}% discount")]
        public void GivenAValidPromoCodeWithDiscount(string code, int discount)
        {
            _promoCode = new PromoCode(code, discount, DateTimeOffset.UtcNow.AddDays(5));
        }

        [When("I check if the promo code is valid today")]
        public void WhenICheckIfThePromoCodeIsValidToday()
        {
            try
            {
                bool isValid = _promoCode.IsValidAt(DateTimeOffset.UtcNow);
                if (!isValid)
                    throw new InvalidOperationException("Promo code is not valid");
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then("the promo code should be marked as invalid")]
        public void ThenThePromoCodeShouldBeMarkedAsInvalid()
        {
            _promoCode.IsValidAt(DateTimeOffset.UtcNow).Should().BeFalse();
        }

        [Then("the promo code creation should fail with error {string}")]
        [Then("the system should reject it with the error {string}")]
        public void ThenTheSystemShouldRejectItWithTheError(string expectedErrorMessage)
        {
            _caughtException.Should().NotBeNull("expected an exception to be thrown, but none occurred.");
            _caughtException.Message.Should().Contain(expectedErrorMessage);
        }

        [Then("the promo code should be created successfully")]
        public void ThenThePromoCodeShouldBeCreatedSuccessfully()
        {
            _promoCode.Should().NotBeNull();
            _caughtException.Should().BeNull();
        }

        [Then("it should be valid today")]
        [Then("and it should be valid today")]
        public void ThenItShouldBeValidToday()
        {
            _promoCode.IsValidAt(DateTimeOffset.UtcNow).Should().BeTrue();
        }

        // --- Context: Applying Promo Code to Cart ---

        [When("I try to apply the promo code to the cart")]
        [When("I apply the promo code to the cart")]
        public void WhenIApplyThePromoCodeToTheCart()
        {
            try
            {
                _cart.ApplyPromoCode(_promoCode, DateTimeOffset.UtcNow);
                _calculatedTotal = _cart.CalculateTotal();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }
    }
}