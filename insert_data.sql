-- Sample data for spicy_noodle_db
USE spicy_noodle_db;

-- Clear existing data (in order to avoid foreign key constraint errors)
-- Delete child tables first, then parent tables
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE order_item_toppings;
TRUNCATE TABLE order_items;
TRUNCATE TABLE order_combos;
TRUNCATE TABLE orders;
TRUNCATE TABLE combo_products;
TRUNCATE TABLE combos;
TRUNCATE TABLE toppings;
TRUNCATE TABLE products;
TRUNCATE TABLE users;
TRUNCATE TABLE daily_revenue;

SET FOREIGN_KEY_CHECKS = 1;

-- Insert Users
INSERT INTO users (username, email, password, full_name, phone, address, role, is_active) VALUES
('admin01', 'admin@spicynoodle.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin User', '0901234567', '123 Nguyen Van Linh, District 7, Ho Chi Minh City', 'admin', TRUE),
('staff01', 'staff@spicynoodle.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Staff Nguyen', '0912345678', '456 Le Van Viet, District 9, Ho Chi Minh City', 'staff', TRUE),
('customer01', 'customer1@gmail.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Tran Van A', '0923456789', '789 Tran Hung Dao, District 1, Ho Chi Minh City', 'customer', TRUE),
('customer02', 'customer2@gmail.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Le Thi B', '0934567890', '321 Vo Thi Sau, District 3, Ho Chi Minh City', 'customer', TRUE),
('customer03', 'customer3@gmail.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Nguyen Van C', '0945678901', '654 Pham Van Dong, Thu Duc City, Ho Chi Minh City', 'customer', TRUE);

-- Insert Products (based on your menu)
INSERT INTO products (product_name, description, base_price, image_url, is_available, spice_level) VALUES
('Mì Kim Chi Bạch Tuộc', 'Mì cay Hàn Quốc với kim chi và bạch tuộc tươi ngon', 50000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/kim-chi-bach-tuot.png', TRUE, 'medium'),
('Mì Kim Chi Bò', 'Mì cay kim chi với thịt bò thái lát mềm ngon', 55000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/mi-kim-chi-bo.png', TRUE, 'medium'),
('Mì Kim Chi Bò Cuộn Nấm', 'Mì cay kim chi với bò cuộn nấm đặc biệt', 55000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/mi-kim-chi-bo-cuon-nam.png', TRUE, 'medium'),
('Mì Kim Chi Thập Cẩm', 'Mì cay kim chi với nhiều loại topping thập cẩm', 65000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/mi-kim-chi-thap-cam.png', TRUE, 'hot'),
('Mì Lẩu Thái Hải Sản', 'Mì cay lẩu Thái với hải sản tươi sống', 55000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/lau-thai-hai-san-Photoroom.png', TRUE, 'hot'),
('Mì Lẩu Thái Cá Hồi', 'Mì cay lẩu Thái với cá hồi tươi cao cấp', 80000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/lau-thai-ca-hoi-Photoroom.png', TRUE, 'hot'),
('Mì Lẩu Thái Xúc Xích', 'Mì cay lẩu Thái với xúc xích và cá viên', 50000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/10/Mi-LT-Xuc-xich-ca-vien-2024-copy.png', TRUE, 'medium'),
('Mì Lẩu Thái Thập Cẩm', 'Mì cay lẩu Thái với nhiều loại topping thập cẩm', 65000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/mi-lau-thai-thap-cam-Photoroom.png', TRUE, 'extra_hot');

-- Insert Toppings
INSERT INTO toppings (topping_name, price, description, is_available) VALUES
('Trứng Cút', 5000.00, 'Trứng cút luộc chín', TRUE),
('Xúc Xích', 10000.00, 'Xúc xích Hàn Quốc', TRUE),
('Cá Viên', 8000.00, 'Cá viên tươi ngon', TRUE),
('Chả Cá', 12000.00, 'Chả cá thái lát', TRUE),
('Rau Muống', 5000.00, 'Rau muống tươi', TRUE),
('Nấm Kim Châm', 8000.00, 'Nấm kim châm tươi', TRUE),
('Đậu Phụ', 6000.00, 'Đậu phụ non', TRUE),
('Thịt Bò Thái', 15000.00, 'Thịt bò thái lát mỏng', TRUE),
('Tôm Tươi', 20000.00, 'Tôm tươi sống', TRUE),
('Mực Tươi', 18000.00, 'Mực tươi thái khoanh', TRUE);

-- Insert Combos
INSERT INTO combos (combo_name, description, combo_price, image_url, is_available) VALUES
('Combo Kim Chi Couple', 'Combo 2 tô mì kim chi + 2 nước ngọt', 100000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/combo-couple.png', TRUE),
('Combo Lẩu Thái Family', 'Combo 4 tô mì lẩu Thái + 4 nước ngọt + topping', 220000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/combo-family.png', TRUE),
('Combo Seoul Special', 'Combo mì kim chi thập cẩm + mì lẩu thái + 2 nước', 125000.00, 'https://micayseoul.com.vn/wp-content/uploads/2024/07/combo-special.png', TRUE);

-- Insert Combo Products relationships
INSERT INTO combo_products (combo_id, product_id, quantity) VALUES
-- Combo Kim Chi Couple (2 mì kim chi bò)
(1, 2, 2),
-- Combo Lẩu Thái Family (4 mì lẩu thái hải sản)
(2, 5, 4),
-- Combo Seoul Special (1 mì kim chi thập cẩm + 1 mì lẩu thái)
(3, 4, 1),
(3, 5, 1);

-- Insert Sample Orders
INSERT INTO orders (user_id, order_status, total_amount, payment_method, payment_status, delivery_address, notes, order_date, confirmed_at) VALUES
(3, 'delivered', 75000.00, 'cash', 'paid', '789 Tran Hung Dao, District 1, Ho Chi Minh City', 'Ít cay hơn', '2024-12-01 12:30:00', '2024-12-01 12:35:00'),
(4, 'delivered', 100000.00, 'digital_wallet', 'paid', '321 Vo Thi Sau, District 3, Ho Chi Minh City', 'Giao nhanh', '2024-12-01 18:45:00', '2024-12-01 18:50:00'),
(5, 'preparing', 180000.00, 'card', 'paid', '654 Pham Van Dong, Thu Duc City, Ho Chi Minh City', 'Thêm rau', '2024-12-02 19:20:00', '2024-12-02 19:25:00'),
(3, 'pending', 125000.00, 'cash', 'pending', '789 Tran Hung Dao, District 1, Ho Chi Minh City', '', '2024-12-02 20:10:00', NULL);

-- Insert Order Items
INSERT INTO order_items (order_id, product_id, quantity, unit_price, subtotal) VALUES
-- Order 1: Mì Kim Chi Bò + toppings
(1, 2, 1, 55000.00, 55000.00),
-- Order 2: Combo Kim Chi Couple
-- (Will be handled in order_combos table)
-- Order 3: Multiple items
(3, 6, 1, 80000.00, 80000.00),
(3, 1, 1, 50000.00, 50000.00),
-- Order 4: Combo Seoul Special
-- (Will be handled in order_combos table)
(4, 8, 1, 65000.00, 65000.00);

-- Insert Order Item Toppings
INSERT INTO order_item_toppings (order_item_id, topping_id, quantity, unit_price, subtotal) VALUES
-- Order 1, Item 1: Add extra toppings
(1, 1, 2, 5000.00, 10000.00),  -- Trứng cút
(1, 8, 1, 15000.00, 15000.00), -- Thịt bò thái
-- Order 3, Item 1: Add seafood toppings
(2, 9, 1, 20000.00, 20000.00), -- Tôm tươi
(2, 10, 1, 18000.00, 18000.00), -- Mực tươi
-- Order 3, Item 2: Add vegetable toppings
(3, 5, 1, 5000.00, 5000.00),   -- Rau muống
(3, 6, 1, 8000.00, 8000.00);   -- Nấm kim châm

-- Insert Order Combos
INSERT INTO order_combos (order_id, combo_id, quantity, unit_price, subtotal) VALUES
-- Order 2: Combo Kim Chi Couple
(2, 1, 1, 100000.00, 100000.00),
-- Order 4: Combo Seoul Special
(4, 3, 1, 125000.00, 125000.00);

-- Insert Daily Revenue
INSERT INTO daily_revenue (revenue_date, total_orders, total_revenue, cash_revenue, card_revenue, digital_wallet_revenue) VALUES
('2024-12-01', 2, 175000.00, 75000.00, 0.00, 100000.00),
('2024-12-02', 2, 305000.00, 125000.00, 180000.00, 0.00),
('2024-11-30', 5, 420000.00, 180000.00, 140000.00, 100000.00),
('2024-11-29', 3, 285000.00, 85000.00, 100000.00, 100000.00),
('2024-11-28', 7, 630000.00, 230000.00, 200000.00, 200000.00);

-- Update order totals to match calculated amounts
UPDATE orders SET total_amount = 80000.00 WHERE order_id = 1; -- 55k + 10k + 15k
UPDATE orders SET total_amount = 100000.00 WHERE order_id = 2; -- Combo price
UPDATE orders SET total_amount = 181000.00 WHERE order_id = 3; -- 80k + 50k + 20k + 18k + 5k + 8k
UPDATE orders SET total_amount = 190000.00 WHERE order_id = 4; -- 65k + 125k

-- Update daily revenue to match corrected order totals
UPDATE daily_revenue SET total_revenue = 180000.00, cash_revenue = 80000.00 WHERE revenue_date = '2024-12-01';
UPDATE daily_revenue SET total_revenue = 371000.00, cash_revenue = 190000.00, card_revenue = 181000.00 WHERE revenue_date = '2024-12-02';