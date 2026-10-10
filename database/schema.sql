CREATE TABLE IF NOT EXISTS carts (
    cart_id UUID PRIMARY KEY,
    applied_promo_code VARCHAR(50),
    updated_at TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS cart_items (
    id SERIAL PRIMARY KEY,
    cart_id UUID NOT NULL REFERENCES carts(cart_id) ON DELETE CASCADE,
    product_id VARCHAR(100) NOT NULL,
    quantity INT NOT NULL,
    unit_price NUMERIC(18, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS promo_codes (
    code VARCHAR(50) PRIMARY KEY,
    discount_percentage NUMERIC(5, 2) NOT NULL,
    expiration_date TIMESTAMP NOT NULL,
    is_active BOOLEAN NOT NULL
);