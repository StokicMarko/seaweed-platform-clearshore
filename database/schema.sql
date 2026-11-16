CREATE SCHEMA IF NOT EXISTS sep3;
SET search_path TO sep3;

CREATE TABLE app_user
(
    id            SERIAL PRIMARY KEY,
    username      VARCHAR(50)  NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name     VARCHAR(100) NOT NULL,
    role          VARCHAR(20)  NOT NULL CHECK (role IN ('STAFF', 'MAIN_ADMIN')),
    is_active     BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE machine
(
    id   SERIAL PRIMARY KEY,
    code VARCHAR(50)  NOT NULL UNIQUE CHECK (code ~ '^[a-z0-9-]+$'),
    name VARCHAR(100) NOT NULL
);

CREATE TABLE country
(
    code    VARCHAR(2) PRIMARY KEY CHECK (code ~ '^[A-Z]{2}$'),
    name_en VARCHAR(100) NOT NULL,
    name_es VARCHAR(100) NOT NULL,
    name_fr VARCHAR(100) NOT NULL
);

CREATE TABLE media
(
    id                 SERIAL PRIMARY KEY,
    machine_id         INT REFERENCES machine (id),
    media_type         VARCHAR(10)  NOT NULL CHECK (media_type IN ('IMAGE', 'VIDEO', 'MODEL_3D')),
    title_en           VARCHAR(200) NOT NULL,
    title_es           VARCHAR(200) NOT NULL,
    title_fr           VARCHAR(200) NOT NULL,
    description_en     TEXT,
    description_es     TEXT,
    description_fr     TEXT,
    original_file_name VARCHAR(255) NOT NULL,
    stored_file_name   VARCHAR(255) NOT NULL UNIQUE,
    content_type       VARCHAR(100) NOT NULL,
    published_at       TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at         TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CONSTRAINT media_description_all_or_none CHECK (
        (description_en IS NULL) = (description_es IS NULL)
            AND (description_en IS NULL) = (description_fr IS NULL))
);

CREATE TABLE news
(
    id           SERIAL PRIMARY KEY,
    title_en     VARCHAR(200) NOT NULL,
    title_es     VARCHAR(200) NOT NULL,
    title_fr     VARCHAR(200) NOT NULL,
    body_en      TEXT         NOT NULL,
    body_es      TEXT         NOT NULL,
    body_fr      TEXT         NOT NULL,
    published_at TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at   TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE news_media
(
    news_id  INT NOT NULL REFERENCES news (id) ON DELETE CASCADE,
    media_id INT NOT NULL REFERENCES media (id) ON DELETE RESTRICT,
    position INT NOT NULL CHECK (position >= 0),
    PRIMARY KEY (news_id, media_id)
);

CREATE TABLE country_deployment
(
    id             SERIAL PRIMARY KEY,
    country_code   VARCHAR(2)    NOT NULL UNIQUE REFERENCES country (code),
    mesh_km        NUMERIC(8, 2) NOT NULL CHECK (mesh_km >= 0),
    device_count   INT           NOT NULL CHECK (device_count >= 0),
    photo_media_id INT REFERENCES media (id) ON DELETE RESTRICT,
    info_en        TEXT          NOT NULL,
    info_es        TEXT          NOT NULL,
    info_fr        TEXT          NOT NULL,
    published_at   TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at     TIMESTAMPTZ   NOT NULL DEFAULT now()
);

CREATE TABLE reference_adjustment
(
    id            SERIAL PRIMARY KEY,
    mesh_km_delta NUMERIC(8, 2) NOT NULL DEFAULT 0,
    device_delta  INT           NOT NULL DEFAULT 0,
    reason        TEXT          NOT NULL,
    published_at  TIMESTAMPTZ   NOT NULL DEFAULT now(),
    CONSTRAINT reference_adjustment_not_empty CHECK (mesh_km_delta <> 0 OR device_delta <> 0)
);

CREATE TABLE change_request
(
    id           SERIAL PRIMARY KEY,
    content_type VARCHAR(30) NOT NULL CHECK (content_type IN
                                             ('NEWS', 'MEDIA', 'COUNTRY_DEPLOYMENT', 'REFERENCE_ADJUSTMENT')),
    action       VARCHAR(10) NOT NULL CHECK (action IN ('CREATE', 'UPDATE', 'DELETE')),
    target_id    INT,
    payload      JSONB,
    status       VARCHAR(10) NOT NULL DEFAULT 'DRAFT' CHECK (status IN ('DRAFT', 'PENDING', 'PUBLISHED', 'REJECTED')),
    requested_by INT         NOT NULL REFERENCES app_user (id),
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    submitted_at TIMESTAMPTZ,
    reviewed_by  INT REFERENCES app_user (id),
    reviewed_at  TIMESTAMPTZ,
    review_note  TEXT,
    CONSTRAINT change_request_payload_unless_delete CHECK ((action = 'DELETE') = (payload IS NULL)),
    CONSTRAINT change_request_target_unless_create CHECK (action = 'CREATE' OR target_id IS NOT NULL),
    CONSTRAINT change_request_published_has_target CHECK (status <> 'PUBLISHED' OR target_id IS NOT NULL),
    CONSTRAINT change_request_submitted_unless_draft CHECK ((status = 'DRAFT') = (submitted_at IS NULL)),
    CONSTRAINT change_request_reviewed_when_decided CHECK (
        (status IN ('PUBLISHED', 'REJECTED')) = (reviewed_by IS NOT NULL AND reviewed_at IS NOT NULL))
);

CREATE UNIQUE INDEX change_request_one_pending_per_item
    ON change_request (content_type, target_id)
    WHERE status = 'PENDING' AND target_id IS NOT NULL;

CREATE VIEW reference_totals AS
SELECT (SELECT COALESCE(SUM(mesh_km), 0) FROM country_deployment)
           + (SELECT COALESCE(SUM(mesh_km_delta), 0) FROM reference_adjustment) AS total_mesh_km,
       (SELECT COALESCE(SUM(device_count), 0) FROM country_deployment)
           + (SELECT COALESCE(SUM(device_delta), 0) FROM reference_adjustment)  AS total_devices,
       (SELECT COUNT(*) FROM country_deployment)                                AS country_count;
