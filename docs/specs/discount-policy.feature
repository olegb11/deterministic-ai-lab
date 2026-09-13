Feature: Discount Policy Calculation
  In order to prevent financial calculation corruptions
  As a Shopping Cart domain
  I want to calculate cart totals and reject invalid items upon creation

  Scenario Outline: Reject creation of item with invalid parameters
    When I try to create an item with price <price>, quantity <quantity>, and discount <discount>
    Then the system should reject it with the error "<error_message>"

    Examples:
      | price | quantity | discount | error_message                  |
      | -10.0 | 1        | 0        | Price cannot be negative       |
      | 100.0 | -1       | 0        | Quantity must be at least 1    |
      | 100.0 | 1        | -5       | Discount cannot be negative    |
      | 100.0 | 1        | 105      | Discount cannot exceed 100%    |

  Scenario: Empty shopping cart total is zero
    Given an empty shopping cart
    When I calculate the total price
    Then the total price should be 0.0

  Scenario Outline: Calculate item total with percentage discount
    When I add an item with price <price>, quantity <quantity>, and discount <discount>
    Then the calculated item total should be <expected_total>

    Examples:
      | price | quantity | discount | expected_total |
      | 100.0 | 1        | 0        | 100.0          |
      | 100.0 | 1        | 10       | 90.0           |
      | 50.0  | 2        | 20       | 80.0           |