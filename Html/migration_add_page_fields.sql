-- Migration: Add new fields to pages table for enhanced page structure
-- Date: 2024-01-30
-- Description: Adds eyebrow, hero_title, hero_text, and last_updated columns to support modern page layouts

-- Add new columns to pages table
ALTER TABLE pages
ADD COLUMN IF NOT EXISTS eyebrow VARCHAR(200),
ADD COLUMN IF NOT EXISTS hero_title VARCHAR(500),
ADD COLUMN IF NOT EXISTS hero_text TEXT,
ADD COLUMN IF NOT EXISTS last_updated VARCHAR(100);

-- Update existing privacy-policy page with new structure
UPDATE pages
SET 
    eyebrow = 'Privacy Policy',
    hero_title = 'How TaxHelperToday handles your information.',
    hero_text = 'This page explains what data we collect, why we collect it, and how you can manage your preferences. Content on this page is fully manageable through the backend.',
    last_updated = 'January 2024'
WHERE slug = 'privacy-policy';
