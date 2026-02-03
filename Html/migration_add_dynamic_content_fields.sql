-- Migration: Add dynamic content fields to pages and services tables
-- This migration adds fields to make static content dynamic and manageable from admin panel

-- Add About page section heading fields
ALTER TABLE pages ADD COLUMN IF NOT EXISTS our_story_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS how_we_work_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS who_we_serve_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS what_we_stand_for_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS team_section_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS team_section_subtitle VARCHAR(500);

-- Add Home page mini-enquiry card fields
ALTER TABLE pages ADD COLUMN IF NOT EXISTS mini_enquiry_badge VARCHAR(100);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS mini_enquiry_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS mini_enquiry_description TEXT;
ALTER TABLE pages ADD COLUMN IF NOT EXISTS mini_enquiry_button_text VARCHAR(100);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS mini_enquiry_note VARCHAR(500);

-- Add Contact page form section fields
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_form_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_form_description TEXT;
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_form_button_text VARCHAR(100);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_form_note VARCHAR(500);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_reach_us_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_find_us_title VARCHAR(200);
ALTER TABLE pages ADD COLUMN IF NOT EXISTS contact_find_us_description TEXT;

-- Add Service enquiry section fields
ALTER TABLE services ADD COLUMN IF NOT EXISTS enquiry_title VARCHAR(200);
ALTER TABLE services ADD COLUMN IF NOT EXISTS enquiry_subtitle TEXT;
ALTER TABLE services ADD COLUMN IF NOT EXISTS enquiry_button_text VARCHAR(100);
ALTER TABLE services ADD COLUMN IF NOT EXISTS enquiry_note VARCHAR(500);

-- Add comments for documentation
COMMENT ON COLUMN pages.our_story_title IS 'Section heading for "Our Story" section on About page';
COMMENT ON COLUMN pages.how_we_work_title IS 'Section heading for "How We Work" section on About page';
COMMENT ON COLUMN pages.who_we_serve_title IS 'Section heading for "Who We Serve" section on About page';
COMMENT ON COLUMN pages.what_we_stand_for_title IS 'Section heading for "What We Stand For" section on About page';
COMMENT ON COLUMN pages.team_section_title IS 'Section heading for "Team" section on About page';
COMMENT ON COLUMN pages.team_section_subtitle IS 'Subtitle/description for "Team" section on About page';
COMMENT ON COLUMN pages.mini_enquiry_badge IS 'Badge text for mini-enquiry card on Home page (e.g., "Free Consultation")';
COMMENT ON COLUMN pages.mini_enquiry_title IS 'Title for mini-enquiry card on Home page';
COMMENT ON COLUMN pages.mini_enquiry_description IS 'Description text for mini-enquiry card on Home page';
COMMENT ON COLUMN pages.mini_enquiry_button_text IS 'Button text for mini-enquiry card on Home page';
COMMENT ON COLUMN pages.mini_enquiry_note IS 'Note text below mini-enquiry form on Home page';
COMMENT ON COLUMN pages.contact_form_title IS 'Title for contact form section on Contact page';
COMMENT ON COLUMN pages.contact_form_description IS 'Description for contact form section on Contact page';
COMMENT ON COLUMN pages.contact_form_button_text IS 'Button text for contact form on Contact page';
COMMENT ON COLUMN pages.contact_form_note IS 'Note text below contact form on Contact page';
COMMENT ON COLUMN pages.contact_reach_us_title IS 'Title for "Reach us" section on Contact page';
COMMENT ON COLUMN pages.contact_find_us_title IS 'Title for "Find us" section on Contact page';
COMMENT ON COLUMN pages.contact_find_us_description IS 'Description for "Find us" section on Contact page';
COMMENT ON COLUMN services.enquiry_title IS 'Title for service enquiry section on Service Detail page';
COMMENT ON COLUMN services.enquiry_subtitle IS 'Subtitle/description for service enquiry section on Service Detail page';
COMMENT ON COLUMN services.enquiry_button_text IS 'Button text for service enquiry form on Service Detail page';
COMMENT ON COLUMN services.enquiry_note IS 'Note text below service enquiry form on Service Detail page';
