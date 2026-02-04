-- TaxHelperToday Database Schema
-- PostgreSQL 15+
-- Run this script to create the complete database structure

-- ============================================
-- IDENTITY MODULE
-- ============================================

-- Users table
CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    is_email_verified BOOLEAN DEFAULT false,
    email_verification_token VARCHAR(500),
    password_reset_token VARCHAR(500),
    password_reset_expires TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_is_active ON users(is_active);

-- Roles table
CREATE TABLE roles (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert default roles
INSERT INTO roles (name, description) VALUES
    ('SuperAdmin', 'Full system access'),
    ('Admin', 'Content and enquiry management'),
    ('Editor', 'Content management only'),
    ('Support', 'Contact enquiries only');

-- User Roles (Many-to-Many)
CREATE TABLE user_roles (
    user_id BIGINT REFERENCES users(id) ON DELETE CASCADE,
    role_id BIGINT REFERENCES roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);

-- Refresh Tokens
CREATE TABLE refresh_tokens (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) UNIQUE NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_revoked BOOLEAN DEFAULT false
);

CREATE INDEX idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_expires_at ON refresh_tokens(expires_at);

-- ============================================
-- CONTENT MODULE
-- ============================================

-- Blog Posts
CREATE TABLE blog_posts (
    id BIGSERIAL PRIMARY KEY,
    slug VARCHAR(255) UNIQUE NOT NULL,
    title VARCHAR(500) NOT NULL,
    excerpt TEXT,
    content TEXT NOT NULL,
    category VARCHAR(100),
    read_time VARCHAR(50),
    featured_image_url VARCHAR(500),
    meta_description VARCHAR(500),
    meta_keywords VARCHAR(500),
    is_published BOOLEAN DEFAULT false,
    published_at TIMESTAMP,
    created_by BIGINT REFERENCES users(id),
    updated_by BIGINT REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    view_count INTEGER DEFAULT 0
);

CREATE INDEX idx_blog_posts_slug ON blog_posts(slug);
CREATE INDEX idx_blog_posts_category ON blog_posts(category);
CREATE INDEX idx_blog_posts_is_published ON blog_posts(is_published);
CREATE INDEX idx_blog_posts_published_at ON blog_posts(published_at);

-- Blog Tags
CREATE TABLE blog_tags (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    slug VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_blog_tags_slug ON blog_tags(slug);

-- Blog Post Tags (Many-to-Many)
CREATE TABLE blog_post_tags (
    blog_post_id BIGINT REFERENCES blog_posts(id) ON DELETE CASCADE,
    tag_id BIGINT REFERENCES blog_tags(id) ON DELETE CASCADE,
    PRIMARY KEY (blog_post_id, tag_id)
);

-- Services
CREATE TABLE services (
    id BIGSERIAL PRIMARY KEY,
    slug VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(500) NOT NULL,
    description TEXT,
    content TEXT,
    type VARCHAR(100), -- Income Tax, GST, TDS, Planning, Support
    level VARCHAR(50), -- Semi Dynamic, Dynamic
    highlight TEXT,
    icon_url VARCHAR(500),
    is_active BOOLEAN DEFAULT true,
    display_order INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_services_slug ON services(slug);
CREATE INDEX idx_services_type ON services(type);
CREATE INDEX idx_services_is_active ON services(is_active);
CREATE INDEX idx_services_display_order ON services(display_order);

-- FAQs
CREATE TABLE faqs (
    id BIGSERIAL PRIMARY KEY,
    question TEXT NOT NULL,
    answer TEXT NOT NULL,
    category VARCHAR(100),
    display_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_faqs_category ON faqs(category);
CREATE INDEX idx_faqs_is_active ON faqs(is_active);
CREATE INDEX idx_faqs_display_order ON faqs(display_order);

-- Static Pages (About, Privacy, Terms, Trust & Safety)
CREATE TABLE pages (
    id BIGSERIAL PRIMARY KEY,
    slug VARCHAR(255) UNIQUE NOT NULL,
    title VARCHAR(500) NOT NULL,
    eyebrow VARCHAR(200),
    hero_title VARCHAR(500),
    hero_text TEXT,
    content TEXT NOT NULL,
    meta_description VARCHAR(500),
    meta_keywords VARCHAR(500),
    last_updated VARCHAR(100),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_pages_slug ON pages(slug);
CREATE INDEX idx_pages_is_active ON pages(is_active);

-- ============================================
-- CONTACT MODULE
-- ============================================

-- Contact Enquiries
CREATE TABLE contact_enquiries (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    subject VARCHAR(500),
    message TEXT NOT NULL,
    enquiry_type VARCHAR(50), -- General, Service, Mini Enquiry
    service_id BIGINT REFERENCES services(id),
    status VARCHAR(50) DEFAULT 'Pending', -- Pending, In Progress, Resolved, Closed
    assigned_to BIGINT REFERENCES users(id),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_contact_enquiries_status ON contact_enquiries(status);
CREATE INDEX idx_contact_enquiries_enquiry_type ON contact_enquiries(enquiry_type);
CREATE INDEX idx_contact_enquiries_created_at ON contact_enquiries(created_at);
CREATE INDEX idx_contact_enquiries_assigned_to ON contact_enquiries(assigned_to);

-- Mini Enquiries (from homepage)
CREATE TABLE mini_enquiries (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    user_type VARCHAR(50), -- Salaried, Business, Professional, NRI
    status VARCHAR(50) DEFAULT 'New', -- New, Contacted, Converted, Closed
    assigned_to BIGINT REFERENCES users(id),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_mini_enquiries_status ON mini_enquiries(status);
CREATE INDEX idx_mini_enquiries_created_at ON mini_enquiries(created_at);

-- ============================================
-- ADMIN MODULE
-- ============================================

-- Admin Settings
CREATE TABLE admin_settings (
    id BIGSERIAL PRIMARY KEY,
    key VARCHAR(100) UNIQUE NOT NULL,
    value TEXT,
    description TEXT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_by BIGINT REFERENCES users(id)
);

-- Insert default settings
INSERT INTO admin_settings (key, value, description) VALUES
    -- Site info
    ('site_name', 'TaxHelperToday', 'Website name'),
    ('site_email', 'mrhoque64@gmail.com', 'Contact email'),
    ('site_phone', '+91-89103-97497', 'Contact phone'),

    -- Contact settings (used across site and banner)
    ('contact_email', 'mrhoque64@gmail.com', 'Contact Email'),
    ('contact_phone', '+91-89103-97497', 'Contact Phone'),
    ('contact_working_hours', 'Monday to Saturday, 10:00 AM – 7:00 PM IST', 'Working Hours'),
    ('contact_whatsapp_number', '918910397497', 'WhatsApp Number (without + or spaces)'),
    ('contact_whatsapp_message', 'Hi, I''d like to get in touch with TaxHelperToday', 'WhatsApp Pre-filled Message'),
    ('contact_office_address', 'TaxHelperToday\n1, Royd Ln, Esplanade\nTaltala, Kolkata, West Bengal - 700016\nIndia', 'Office Address'),
    ('contact_map_location', '1 Royd Ln, Esplanade, Taltala, Kolkata, West Bengal 700016', 'Google Maps Location (address or place name)'),

    -- ITR deadline banner
    ('it_deadline_countdown', 'true', 'Show ITR deadline countdown'),
    ('it_deadline_date', '2024-07-31', 'ITR filing deadline date'),
    ('it_deadline_title', 'ITR Filing Deadline Approaching!', 'ITR deadline banner title'),
    ('it_deadline_text', 'File now to avoid penalties and maximize your refund.', 'ITR deadline banner description (after the days counter).'),
    ('it_deadline_button_text', 'File ITR Now', 'ITR deadline banner button text'),
    ('it_deadline_support_template', 'Need help? Call {{phone}} or email {{email}}.', 'ITR deadline banner support line. Use {{phone}} and {{email}} placeholders.');

-- Activity Logs
CREATE TABLE activity_logs (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT REFERENCES users(id),
    action VARCHAR(100) NOT NULL, -- Created, Updated, Deleted, LoggedIn, etc.
    entity_type VARCHAR(100), -- BlogPost, Service, FAQ, etc.
    entity_id BIGINT,
    details JSONB,
    ip_address VARCHAR(50),
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_activity_logs_user_id ON activity_logs(user_id);
CREATE INDEX idx_activity_logs_entity_type ON activity_logs(entity_type);
CREATE INDEX idx_activity_logs_created_at ON activity_logs(created_at);

-- ============================================
-- FUNCTIONS & TRIGGERS
-- ============================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Triggers for updated_at
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_blog_posts_updated_at BEFORE UPDATE ON blog_posts
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_services_updated_at BEFORE UPDATE ON services
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_faqs_updated_at BEFORE UPDATE ON faqs
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_pages_updated_at BEFORE UPDATE ON pages
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_contact_enquiries_updated_at BEFORE UPDATE ON contact_enquiries
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_mini_enquiries_updated_at BEFORE UPDATE ON mini_enquiries
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- ============================================
-- INITIAL DATA SEEDING
-- ============================================

-- Create default admin user (password: Admin@123 - should be changed after first login)
-- Password hash for "Admin@123" using BCrypt (rounds=12)
-- Note: This is a placeholder. Generate actual hash during user creation.
INSERT INTO users (email, password_hash, full_name, is_active, is_email_verified) VALUES
    ('admin@taxhelpertoday.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5', 'System Administrator', true, true);

-- Assign SuperAdmin role to default admin
INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id
FROM users u, roles r
WHERE u.email = 'admin@taxhelpertoday.com' AND r.name = 'SuperAdmin';

-- ============================================
-- COMMENTS FOR DOCUMENTATION
-- ============================================

COMMENT ON TABLE users IS 'System users including admins and staff';
COMMENT ON TABLE roles IS 'User roles for authorization';
COMMENT ON TABLE blog_posts IS 'Blog articles and guides';
COMMENT ON TABLE services IS 'Tax services offered by the platform';
COMMENT ON TABLE faqs IS 'Frequently asked questions';
COMMENT ON TABLE pages IS 'Static content pages (About, Privacy, Terms, etc.)';
COMMENT ON TABLE contact_enquiries IS 'Contact form submissions';
COMMENT ON TABLE mini_enquiries IS 'Quick enquiries from homepage';
COMMENT ON TABLE activity_logs IS 'Audit trail of user actions';
