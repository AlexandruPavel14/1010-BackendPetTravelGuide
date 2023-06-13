-- Create authentication schema
-- CREATE SCHEMA authentication AUTHORIZATION pavel;

-- Create authentication schema
CREATE SCHEMA IF NOT EXISTS authentication AUTHORIZATION pavel;

-- Roles table
CREATE TABLE IF NOT EXISTS authentication.roles
(
    id smallint NOT NULL GENERATED ALWAYS AS IDENTITY,
    name character varying(256) NOT NULL,
    normalized_name character varying(256) NOT NULL,
    CONSTRAINT roles_pkey PRIMARY KEY (id)
);
ALTER TABLE IF EXISTS authentication.roles OWNER to pavel;

-- This trigger computes normalized_name from name (ex: Administrator -> Administrator, Utilizator Simplu -> UtilizatorSimplu, etc.)
-- The backend only uses normalized names
CREATE OR REPLACE FUNCTION authentication.trgfn_before_insert_or_update_roles()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    NEW.normalized_name := REPLACE(INITCAP(NEW.name), ' ', '');
    RETURN NEW;
END;
$BODY$;
ALTER FUNCTION authentication.trgfn_before_insert_or_update_roles() OWNER TO pavel;

CREATE TRIGGER trg_before_insert_or_update_roles
    BEFORE INSERT OR UPDATE 
    ON authentication.roles
    FOR EACH ROW
    EXECUTE FUNCTION authentication.trgfn_before_insert_or_update_roles();

-- Populate roles table
INSERT INTO authentication.roles(name) VALUES ('Administrator'), ('Reviewer'), ('User');

-- Users table
CREATE TABLE IF NOT EXISTS authentication.users
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    email character varying(256) NOT NULL,
    -- email_confirmed boolean NOT NULL DEFAULT false, -- email confirmation functionality -> to be added later (requires access to SMTP server)
    first_name character varying(32) NOT NULL,
    last_name character varying(32) NOT NULL,
    gender character(1),
    password_hash character varying(128) NOT NULL,
    phone_number character varying(16),
    -- lockout_end timestamp without time zone, -- user locking functionality -> to be added later
    -- lockout_enabled boolean NOT NULL DEFAULT false, -- user locking functionality -> to be added later
    -- access_failed_count integer NOT NULL DEFAULT 0, -- user locking functionality -> to be added later
    -- picture_url character varying(1024), -- user avatar picture -> to be added later
    created_at timestamp without time zone NOT NULL DEFAULT timezone('Europe/Bucharest'::text, now()),
    modified_at timestamp without time zone,
    active boolean NOT NULL DEFAULT true,
    CONSTRAINT users_pkey PRIMARY KEY (id)
);
ALTER TABLE IF EXISTS authentication.users OWNER to pavel;

-- Create index on email column because it is the main identifier of an account
CREATE UNIQUE INDEX IF NOT EXISTS users_email_index ON authentication.users USING btree (email ASC NULLS LAST);

-- UsersRoles table (this is a JOIN table that creates many to many relationships between user and roles table)
CREATE TABLE IF NOT EXISTS authentication.users_roles
(
    user_id bigint NOT NULL,
    role_id smallint NOT NULL,
    CONSTRAINT users_roles_pkey PRIMARY KEY (user_id, role_id),
    CONSTRAINT user_roles_roles_role_id_fkey FOREIGN KEY (role_id)
        REFERENCES authentication.roles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT user_roles_users_user_id FOREIGN KEY (user_id)
        REFERENCES authentication.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);
ALTER TABLE IF EXISTS authentication.users_roles OWNER to pavel;

-- CREATE TABLE public.locations
-- (
--     id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
--     name character varying(256) NOT NULL,
--     description text,
--     type smallint NOT NULL,
--     latitude double precision NOT NULL,
--     longitude double precision NOT NULL,
--     address character varying(256),
--     google_places_id text,
-- 	user_id bigint NOT NULL DEFAULT 1, -- THE DEFAULT VALUE MUST BE THE ID OF THE ADMINISTRATOR ACCOUNT
-- 	rating smallint NOT NULL DEFAULT 0,
--     PRIMARY KEY (id),
-- 	FOREIGN KEY (user_id)
--         REFERENCES authentication.users (id) MATCH SIMPLE
--         ON UPDATE CASCADE
--         ON DELETE SET DEFAULT
-- );

ALTER TABLE IF EXISTS public.locations
    OWNER to pavel;

COMMENT ON COLUMN public.locations.type
    IS '1 - parc, 2 - petshop, 3 - veterinar, 4 - restaurant, 5 - cazare, 6 - turistic';

CREATE TABLE public.pets
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    user_id bigint NOT NULL,
    name character varying(128) NOT NULL,
    species character varying(128) NOT NULL,
    age interval NOT NULL,
    created_at timestamp without time zone NOT NULL DEFAULT timezone('Europe/Bucharest'::text, now()),
    PRIMARY KEY (id),
    FOREIGN KEY (user_id)
        REFERENCES authentication.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

ALTER TABLE IF EXISTS public.pets
    OWNER to pavel;

-- Create the 'utilizatori' table
CREATE TABLE public.utilizatori (
    id SERIAL PRIMARY KEY,
    firstname VARCHAR,
    lastname VARCHAR
);

-- Create the 'item' table
CREATE TABLE public.item (
    id SERIAL PRIMARY KEY,
    country VARCHAR,
    county VARCHAR,
    city VARCHAR,
    name VARCHAR,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    rating DOUBLE PRECISION,
    userratingstotal INT,
    onestar INT,
    twostar INT,
    threestar INT,
    fourstar INT,
    fivestar INT,
    mountains INT,
    hills INT,
    plains INT,
    plateaus INT,
    valleys INT,
    glacialfields INT,
    deltas INT,
    canyons INT,
    beaches INT,
    naturespots INT,
    stunningviews INT,
    lakes INT,
    parks INT,
    iconiccities INT,
    farms INT,
    castles INT,
    historicalhomes INT,
    boatrides INT,
    lakefrontareas INT,
    swimmingareas INT,
    caves INT,
    playgrounds INT
);

-- Create the 'data' table
CREATE TABLE public.data (
    id SERIAL PRIMARY KEY,
    user_id INT,
    location_id INT,
    rating DOUBLE PRECISION,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES public.utilizatori (id),
    CONSTRAINT fk_location FOREIGN KEY (location_id) REFERENCES public.item (id)
);