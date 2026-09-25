using System;
using Reqnroll;
using FluentAssertions;
using DeterministicAiLab.Domain.Entities;

namespace DeterministicAiLab.Domain.BDD.Tests
{
    [Binding]
    public class PromoCodePolicyStepDefinitions
    {
        private PromoCode? _promo;
        private bool _validToday;
        private Exception? _creationException;

        [BeforeScenario]
        public void BeforeScenario()
        {
            _promo = null;
            _validToday = false;
            _creationException = null;
        }

        [When("I create a promo code with code {string}, discount {int}%, and expiration date in {int} days")]
        public void WhenICreateAPromoCodeWithCodeDiscountAndExpirationDateInDays(string code, int discount, int days)
        {
            _creationException = null;
            try
            {
                var now = DateTimeOffset.UtcNow;
                _promo = new PromoCode(code, discount, now.AddDays(days), now);
            }
            catch (Exception ex)
            {
                _creationException = ex;
                _promo = null;
            }
        }

        [Then("the promo code should be created successfully")]
        public void ThenThePromoCodeShouldBeCreatedSuccessfully()
        {
            _creationException.Should().BeNull();
            _promo.Should().NotBeNull();
        }

        [Then("it should be valid today")]
        public void ThenItShouldBeValidToday()
        {
            _promo.Should().NotBeNull();
            _promo!.IsValid(DateTimeOffset.UtcNow).Should().BeTrue();
        }

        [When("I create a promo code with code {string} and discount {int}%")]
        public void WhenICreateAPromoCodeWithCodeAndDiscount(string code, int discount)
        {
            _creationException = null;
            _promo = null;
            try
            {
                // create with a default expiration (now) to exercise validation around code/discount
                var now = DateTimeOffset.UtcNow;
                _promo = new PromoCode(code, discount, now, now);
            }
            catch (Exception ex)
            {
                _creationException = ex;
            }
        }

        [Then("the promo code creation should fail with error {string}")]
        public void ThenThePromoCodeCreationShouldFailWithError(string expectedMessage)
        {
            _creationException.Should().NotBeNull();
            _creationException!.Message.Should().Contain(expectedMessage);
        }

        [Given("a promo code {string} with {int}% discount that expired {int} day ago")]
        public void GivenAPromoCodeWithDiscountThatExpiredDayAgo(string code, int discount, int daysAgo)
        {
            var now = DateTimeOffset.UtcNow;
            _promo = new PromoCode(code, discount, now.AddDays(-daysAgo), now.AddDays(-daysAgo - 1));
            _creationException = null;
        }

        [When("I check if the promo code is valid today")]
        public void WhenICheckIfThePromoCodeIsValidToday()
        {
            _promo.Should().NotBeNull();
            _validToday = _promo!.IsValid(DateTimeOffset.UtcNow);
        }

        [Then("the promo code should be marked as invalid")]
        public void ThenThePromoCodeShouldBeMarkedAsInvalid()
        {
            _validToday.Should().BeFalse();
        }

    }
}
