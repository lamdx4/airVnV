-- Create Admin User for AirVnV
-- Run this against the UserService PostgreSQL database

DO $$
DECLARE
    admin_id UUID := gen_random_uuid();
BEGIN
    -- Check if admin already exists
    IF EXISTS (SELECT 1 FROM users WHERE email = 'admin@airvnv.com') THEN
        RAISE NOTICE 'Admin user already exists';
    ELSE
        -- Insert admin user
        INSERT INTO users (id, email, hashed_password, role, status, kyc_status, created_at, updated_at)
        VALUES (
            admin_id,
            'admin@airvnv.com',
            'admin123',  -- Plain text password (dev mode)
            'Admin',
            'Active',
            'NotSubmitted',
            NOW(),
            NOW()
        );

        -- Insert admin profile
        INSERT INTO user_profiles (user_id, full_name, bio, created_at, updated_at)
        VALUES (admin_id, 'Admin User', 'System Administrator', NOW(), NOW());

        RAISE NOTICE 'Admin user created: admin@airvnv.com / admin123';
    END IF;
END $$;
