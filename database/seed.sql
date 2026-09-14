-- =====================================================================
-- Seed Data for Testing & Development
-- =====================================================================

-- 1. Insert sample catalog items (products)
INSERT INTO items (code, name, price, description, is_active) VALUES
('ITEM-1001', 'Keyboard Mechanic', 100.00, 'RGB Mechanical Gaming Keyboard', TRUE),
('ITEM-1002', 'Wireless Mouse', 50.00, 'Ergonomic 2.4GHz Wireless Mouse', TRUE),
('ITEM-1003', 'Gaming Headset', 120.00, '7.1 Surround Sound Noise Canceling Headset', TRUE),
('ITEM-1004', 'Discontinued Item', 30.00, 'Legacy Product No Longer Sold', FALSE);

-- 2. Insert sample promo codes (valid and expired based on 2026 timeline)
INSERT INTO promo_codes (code, discount_percentage, expiration_date) VALUES
('SAVE10', 10, '2026-12-31 23:59:59+00'), -- Active valid 10% promo code[cite: 4]
('HALF20', 50, '2026-12-31 23:59:59+00'), -- Active valid 50% promo code[cite: 4]
('OLDD10', 20, '2025-01-01 00:00:00+00'); -- Expired promo code for testing rejection logic[cite: 4]

-- 3. Insert sample shopping carts
-- Cart 1: Has an active promo code applied ('SAVE10')[cite: 4]
-- Cart 2: No promo code applied[cite: 4]
INSERT INTO carts (id, applied_promo_code) VALUES
('11111111-1111-1111-1111-111111111111', 'SAVE10'),
('22222222-2222-2222-2222-222222222222', NULL);

-- 4. Insert items for Cart 1 
-- Total calculation test: ITEM-1001 (Keyboard Mechanic) -> 100.00 * 2 items = 200.00, minus 10% promo = 180.00
INSERT INTO cart_items (cart_id, item_code, price, quantity, discount) VALUES
('11111111-1111-1111-1111-111111111111', 'ITEM-1001', 100.00, 2, 0);

-- 5. Insert items for Cart 2 
-- Item with percentage discount test: ITEM-1002 (Wireless Mouse) -> 50.00 * 2 items with 20% item discount = 80.00
INSERT INTO cart_items (cart_id, item_code, price, quantity, discount) VALUES
('22222222-2222-2222-2222-222222222222', 'ITEM-1002', 50.00, 2, 20);