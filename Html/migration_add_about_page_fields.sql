-- Migration: Add new fields to pages table for About page structured data
-- Date: 2024-02-02
-- Description: Adds JSON and text columns to support dynamic About page content

-- Add new columns to pages table
ALTER TABLE pages
ADD COLUMN IF NOT EXISTS stats_json JSONB,
ADD COLUMN IF NOT EXISTS our_story_content TEXT,
ADD COLUMN IF NOT EXISTS how_we_work_content TEXT,
ADD COLUMN IF NOT EXISTS how_we_work_checklist_json JSONB,
ADD COLUMN IF NOT EXISTS mission_cards_json JSONB,
ADD COLUMN IF NOT EXISTS who_we_serve_content TEXT,
ADD COLUMN IF NOT EXISTS who_we_serve_categories_json JSONB,
ADD COLUMN IF NOT EXISTS values_json JSONB,
ADD COLUMN IF NOT EXISTS team_members_json JSONB;

-- Create indexes for JSON columns (optional, for better query performance)
CREATE INDEX IF NOT EXISTS idx_pages_stats_json ON pages USING GIN (stats_json);
CREATE INDEX IF NOT EXISTS idx_pages_mission_cards_json ON pages USING GIN (mission_cards_json);
CREATE INDEX IF NOT EXISTS idx_pages_team_members_json ON pages USING GIN (team_members_json);
