Feature: Shopping Cart and Promo Code Policy
  As a customer
  I want to apply discount promo codes to my shopping cart
  So that the total price is calculated accurately according to business rules

  @invariant:M1 @invariant:M2 @invariant:P1
  Scenario: Applying a valid 10 percent discount promo code to a cart
    Given a shopping cart with items totaling 1000.00 UAH
    When the user applies the promo code "SAVE10"
    Then the cart subtotal should remain 1000.00 UAH
    And the applied promo code should be "SAVE10"
    And the cart total price should become 900.00 UAH

  @invariant:M1
  Scenario: Attempting to apply an expired or invalid format promo code
    Given a shopping cart with items totaling 500.00 UAH
    When the user attempts to apply an invalid promo code "BAD1"
    Then a domain validation exception should be thrown
    And the cart total price should remain 500.00 UAH