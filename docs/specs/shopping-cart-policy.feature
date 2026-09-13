Feature: Shopping Cart Promo Code Policy
  In order to apply promotional discounts correctly
  As a Shopping Cart domain
  I want to calculate the final total when valid or invalid promo codes are applied

  Scenario: Calculate cart total without promo code
    Given a shopping cart with the following items:
      | price | quantity | discount |
      | 100.0 | 2        | 0        |
    When no promo code is applied
    Then the cart total should be 200.0

  Scenario: Apply valid promo code to cart
    Given a shopping cart with the following items:
      | price | quantity | discount |
      | 100.0 | 2        | 0        |
    And a valid promo code "SAVE10" with 10% discount
    When I apply the promo code to the cart
    Then the cart total should be 180.0

  Scenario: Reject applying invalid or expired promo code
    Given a shopping cart
    And an expired promo code "OLDD10" with 20% discount
    When I try to apply the promo code to the cart
    Then the system should reject it with the error "Cannot apply invalid or expired promo code"