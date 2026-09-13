-- =====================================================================
-- Database Schema: Shopping Cart & Promo Codes
-- Engine: PostgreSQL
-- =====================================================================

-- 1. Promo codes table storing discount rules and expiration limits
CREATE TABLE promo_codes (
    code VARCHAR(6) PRIMARY KEY, -- Unique 6-character promo code identifier
    discount_percentage INT NOT NULL CHECK (discount_percentage BETWEEN 1 AND 100), -- Discount value ranging from 1% to 100%
    expiration_date TIMESTAMPTZ NOT NULL, -- Expiration timestamp of the promo code
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Constraint ensuring exact 6-character length excluding whitespace padding
ALTER TABLE promo_codes ADD CONSTRAINT chk_promo_code_length CHECK (LENGTH(TRIM(code)) = 6);


-- 2. Shopping carts table maintaining state and applied discounts
CREATE TABLE carts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique cart identifier
    applied_promo_code VARCHAR(6) REFERENCES promo_codes(code) ON DELETE SET NULL, -- Optional applied promo code reference
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);


-- 3. Cart items table holding individual product lines, prices, and item-level discounts
CREATE TABLE cart_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique cart item identifier
    cart_id UUID NOT NULL REFERENCES carts(id) ON DELETE CASCADE, -- Parent cart reference with cascade deletion
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0), -- Unit price, must be non-negative
    quantity INT NOT NULL CHECK (quantity >= 1), -- Item quantity, must be at least 1
    discount NUMERIC(5, 2) NOT NULL DEFAULT 0 CHECK (discount >= 0 AND discount <= 100), -- Item-level percentage discount (0-100)
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Performance index for retrieving all items belonging to a specific cart via Dapper
CREATE INDEX idx_cart_items_cart_id ON cart_items(cart_id);