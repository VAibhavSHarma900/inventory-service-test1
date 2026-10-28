-- psql-only bootstrap; database must not already exist.
CREATE DATABASE inventory_db;
\connect inventory_db
\ir 01-schema.sql
