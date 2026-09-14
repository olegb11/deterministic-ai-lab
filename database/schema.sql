-- =====================================================================
-- Database Schema: Shopping Cart & Promo Codes
-- Engine: PostgreSQL
-- =====================================================================

-- 1. Catalog items (products) table
CREATE TABLE items (
    code VARCHAR(20) PRIMARY KEY, -- Unique item code identifier (e.g., ITEM-1001)
    name VARCHAR(50) NOT NULL, -- Item name
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0), -- Base product price, must be non-negative
    description TEXT, -- Detailed item description
    is_active BOOLEAN NOT NULL DEFAULT TRUE, -- Soft-deletion flag to keep legacy cart references intact
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Constraint ensuring item code is not empty or whitespace-only
ALTER TABLE items ADD CONSTRAINT chk_item_code_not_empty CHECK (LENGTH(TRIM(code)) > 0);


-- 2. Promo codes table storing discount rules and expiration limits
CREATE TABLE promo_codes (
    code VARCHAR(6) PRIMARY KEY, -- Unique 6-character promo code identifier[cite: 3]
    discount_percentage INT NOT NULL CHECK (discount_percentage BETWEEN 1 AND 100), -- Discount value ranging from 1% to 100%[cite: 3]
    expiration_date TIMESTAMPTZ NOT NULL, -- Expiration timestamp of the promo code[cite: 3]
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Constraint ensuring exact 6-character length excluding whitespace padding[cite: 3]
ALTER TABLE promo_codes ADD CONSTRAINT chk_promo_code_length CHECK (LENGTH(TRIM(code)) = 6);


-- 3. Shopping carts table maintaining state and applied discounts
CREATE TABLE carts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique cart identifier[cite: 3]
    applied_promo_code VARCHAR(6) REFERENCES promo_codes(code) ON DELETE SET NULL, -- Optional applied promo code reference[cite: 3]
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);


-- 4. Cart items table holding individual product lines, prices, and item-level discounts
CREATE TABLE cart_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique cart item identifier[cite: 3]
    cart_id UUID NOT NULL REFERENCES carts(id) ON DELETE CASCADE, -- Parent cart reference with cascade deletion[cite: 3]
    item_code VARCHAR(20) NOT NULL REFERENCES items(code), -- Foreign key linking line item to the catalog product
    price NUMERIC(18, 2) NOT NULL CHECK (price >= 0), -- Captured unit price at time of adding to cart[cite: 3]
    quantity INT NOT NULL CHECK (quantity >= 1), -- Item quantity, must be at least 1[cite: 3]
    discount NUMERIC(5, 2) NOT NULL DEFAULT 0 CHECK (discount >= 0 AND discount <= 100), -- Item-level percentage discount (0-100)[cite: 3]
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Performance indices for Dapper queries
CREATE INDEX idx_cart_items_cart_id ON cart_items(cart_id);
CREATE INDEX idx_cart_items_item_code ON cart_items(item_code);