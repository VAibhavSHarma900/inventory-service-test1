-- Run against inventory_db, or use create-database.sql for initial setup.
-- Preserve the assessment's table, constraints and indexes.
CREATE TABLE IF NOT EXISTS inventory_items (
    item_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    item_name VARCHAR(150) NOT NULL,
    category VARCHAR(100),
    quantity INT NOT NULL CHECK (quantity >= 0),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_inventory_item_name ON inventory_items(item_name);
CREATE INDEX IF NOT EXISTS idx_inventory_is_active ON inventory_items(is_active);
