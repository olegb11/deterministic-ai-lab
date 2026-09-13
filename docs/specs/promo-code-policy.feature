Feature: Promo Code Policy
  In order to offer promotional discounts
  As a Shopping Cart domain
  I want to validate promo code format, expiration date, and discount range

  Scenario Outline: Create valid promo code
    When I create a promo code with code "<code_val>", discount <discount>%, and expiration date in <days> days
    Then the promo code should be created successfully
    And it should be valid today

  Examples:
    | code_val | discount | days |
    | SAVE10   | 10       | 7    |
    | HALF20   | 50       | 30   |

  Scenario Outline: Reject invalid promo code format
    When I create a promo code with code "<code_val>" and discount <discount>%
    Then the promo code creation should fail with error "<error_message>"

  Examples:
    | code_val | discount | error_message                   |
    |          | 10       | Promo code cannot be empty      |
    | SHORT    | 10       | Promo code must be 6 characters  |
    | TOOLONG1 | 10       | Promo code must be 6 characters  |

  Scenario Outline: Reject invalid discount percentage
    When I create a promo code with code "SAVE10" and discount <discount>%
    Then the promo code creation should fail with error "Discount percentage must be between 1 and 100"

  Examples:
    | discount |
    | 0        |
    | -5       |
    | 101      |

  Scenario: Detect expired promo code
    Given a promo code "SAVE10" with 10% discount that expired 1 day ago
    When I check if the promo code is valid today
    Then the promo code should be marked as invalid