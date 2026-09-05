-- Initialize SimplePubManager database and user

-- Create database
CREATE DATABASE simplepubmanager;

-- Create user
CREATE USER simplepubmanager WITH PASSWORD 'simplepubmanager';

-- Grant privileges
ALTER ROLE simplepubmanager WITH CREATEDB;
ALTER DATABASE simplepubmanager OWNER TO simplepubmanager;

-- Grant all privileges on simplepubmanager database
GRANT ALL PRIVILEGES ON DATABASE simplepubmanager TO simplepubmanager;

-- Connect to simplepubmanager database and grant schema privileges
\c simplepubmanager

GRANT ALL PRIVILEGES ON SCHEMA public TO simplepubmanager;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO simplepubmanager;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO simplepubmanager;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON FUNCTIONS TO simplepubmanager;
