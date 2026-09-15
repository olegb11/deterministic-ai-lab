Feature: Shopping Cart Discounts & Invariants
  In order to ensure financial accuracy
  As a store pricing engine
  I want to apply promo codes while preserving domain invariants

  Scenario: Apply SUMMER2026 promo code for orders strictly above 1000 UAH
    Given a shopping cart with total item amount of 1200 UAH
    When the promo code "SUMMER2026" is applied
    Then the total cart amount should be 1080 UAH
    And the final total must never be negative