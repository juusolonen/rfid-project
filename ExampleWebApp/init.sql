-- init.sql
CREATE ROLE app WITH LOGIN PASSWORD 'yourpassword';
GRANT ALL PRIVILEGES ON DATABASE exampledb TO app;

-- Grant permissions on the public schema and its tables/sequences
GRANT ALL PRIVILEGES ON SCHEMA public TO app;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO app;