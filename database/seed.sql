-- =====================================================================
-- Seed Data for Testing & Development
-- =====================================================================

-- Insert sample promo codes (valid and expired based on current 2026 timeline)
INSERT INTO promo_codes (code, discount_percentage, expiration_date) VALUES
('SAVE10', 10, '2026-12-31 23:59:59+00'), -- Active valid 10% promo code
('HALF20', 50, '2026-12-31 23:59:59+00'), -- Active valid 50% promo code
('OLDD10', 20, '2025-01-01 00:00:00+00'); -- Expired promo code for testing rejection logic

-- Insert sample shopping carts
-- Cart 1: Has an active promo code applied ('SAVE10')
-- Cart 2: No promo code applied
INSERT INTO carts (id, applied_promo_code) VALUES
('11111111-1111-1111-1111-111111111111', 'SAVE10'),
('22222222-2222-2222-2222-222222222222', NULL);

-- Insert items for Cart 1 (Total calculation test: 100.00 * 2 items = 200.00, minus 10% promo = 180.00)
INSERT INTO cart_items (cart_id, price, quantity, discount) VALUES
('11111111-1111-1111-1111-111111111111', 100.00, 2, 0);

-- Insert items for Cart 2 (Item with percentage discount test: 50.00 * 2 items with 20% item discount = 80.00)
INSERT INTO cart_items (cart_id, price, quantity, discount) VALUES
('22222222-2222-2222-2222-222222222222', 50.00, 2, 20);