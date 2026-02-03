-- Migration: Add new fields to pages table for Home page structured data
-- Date: 2024-02-03
-- Description: Adds JSON columns to support dynamic Home page content sections

-- Add new columns to pages table
ALTER TABLE pages
ADD COLUMN IF NOT EXISTS hero_trust_badges_json JSONB,
ADD COLUMN IF NOT EXISTS hero_metrics_json JSONB,
ADD COLUMN IF NOT EXISTS stats_banner_json JSONB,
ADD COLUMN IF NOT EXISTS cta_banner_json JSONB,
ADD COLUMN IF NOT EXISTS how_it_works_json JSONB,
ADD COLUMN IF NOT EXISTS why_section_json JSONB,
ADD COLUMN IF NOT EXISTS pricing_plans_json JSONB,
ADD COLUMN IF NOT EXISTS trust_section_json JSONB,
ADD COLUMN IF NOT EXISTS testimonials_json JSONB,
ADD COLUMN IF NOT EXISTS value_proposition_json JSONB;

-- Create indexes for JSON columns (optional, for better query performance)
CREATE INDEX IF NOT EXISTS idx_pages_hero_trust_badges_json ON pages USING GIN (hero_trust_badges_json);
CREATE INDEX IF NOT EXISTS idx_pages_hero_metrics_json ON pages USING GIN (hero_metrics_json);
CREATE INDEX IF NOT EXISTS idx_pages_stats_banner_json ON pages USING GIN (stats_banner_json);
CREATE INDEX IF NOT EXISTS idx_pages_pricing_plans_json ON pages USING GIN (pricing_plans_json);
CREATE INDEX IF NOT EXISTS idx_pages_testimonials_json ON pages USING GIN (testimonials_json);
