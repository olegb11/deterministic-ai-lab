using System;
using Reqnroll;
using FluentAssertions;
using DeterministicAiLab.Domain.Entities;
using System.Linq;

namespace DeterministicAiLab.Domain.BDD.Tests
{
    [Binding]
    public class ShoppingCartPromoCodePolicyStepDefinitions
    {
        private Cart? _cart;
        private PromoCode? _promo;
        private Exception? _applyException;

        [BeforeScenario]
        public void BeforeScenario()
        {
            _cart = null;
            _promo = null;
            _applyException = null;
        }

        [Given("a shopping cart with the following items:")]
        public void GivenAShoppingCartWithTheFollowingItems(DataTable dataTable)
        {
            _applyException = null;
            _cart = new Cart(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
            // Attempt to read rows in a forgiving way
            foreach (var row in dataTable.Rows)
            {
                // row is IDictionary<string,string>
                var price = Decimal.Parse(row["price"].ToString());
                var quantity = int.Parse(row["quantity"].ToString());
                var discount = int.Parse(row["discount"].ToString());
                var item = new CartItem(Guid.NewGuid(), _cart.Id, "ITEM", price, quantity, discount, DateTimeOffset.UtcNow);
                _cart.AddItem(item);
            }
        }

        [When("no promo code is applied")]
        public void WhenNoPromoCodeIsApplied()
        {
            _promo = null;
            _applyException = null;
        }

        [Then("the cart total should be {float}")]
        public void ThenTheCartTotalShouldBe(Decimal expected)
        {
            _cart.Should().NotBeNull();
            var total = _cart!.Total(_cart.AppliedPromoCode, DateTimeOffset.UtcNow);
            total.Should().BeApproximately(expected, 0.0001m);
        }

        [Given("a valid promo code {string} with {int}% discount")]
        public void GivenAValidPromoCodeWithDiscount(string code, int discount)
        {
            var now = DateTimeOffset.UtcNow;
            _promo = new PromoCode(code, discount, now.AddDays(7), now);
            _applyException = null;
        }

        [When("I apply the promo code to the cart")]
        public void WhenIApplyThePromoCodeToTheCart()
        {
            _applyException = null;
            _cart.Should().NotBeNull();
            try
            {
                _cart!.ApplyPromoCode(_promo);
            }
            catch (Exception ex)
            {
                _applyException = ex;
            }
        }

        [Given("a shopping cart")]
        public void GivenAShoppingCart()
        {
            _cart = new Cart(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
            _applyException = null;
        }

        [Given("an expired promo code {string} with {int}% discount")]
        public void GivenAnExpiredPromoCodeWithDiscount(string code, int discount)
        {
            var now = DateTimeOffset.UtcNow;
            _promo = new PromoCode(code, discount, now.AddDays(-1), now.AddDays(-2));
            _applyException = null;
        }

        [When("I try to apply the promo code to the cart")]
        public void WhenITryToApplyThePromoCodeToTheCart()
        {
            _applyException = null;
            _cart.Should().NotBeNull();
            try
            {
                _cart!.ApplyPromoCode(_promo);
            }
            catch (Exception ex)
            {
                _applyException = ex;
            }
        }

        [Then("applying the promo code should be rejected with the error {string}")]
        public void ThenApplyingThePromoCodeShouldBeRejectedWithTheError(string expectedMessage)
        {
            _applyException.Should().NotBeNull();
            _applyException!.Message.Should().Contain(expectedMessage);
        }

    }
}
