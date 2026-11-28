CREATE SCHEMA IF NOT EXISTS sep3;
SET search_path TO sep3;

-- Not in the ER yet: StaffMember and MainAdmin are identified by email in the docs.
-- Kept as is until the "Users by email" issue. :)
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


CREATE TABLE technology
(
    type VARCHAR(100) PRIMARY KEY
);

CREATE TABLE country
(
    code    VARCHAR(2) PRIMARY KEY CHECK (code ~ '^[A-Z]{2}$'),
    name_en VARCHAR(100) NOT NULL,
    name_es VARCHAR(100) NOT NULL,
    name_fr VARCHAR(100) NOT NULL
);

CREATE TABLE media_item
(
    id           SERIAL PRIMARY KEY,
    type         VARCHAR(10)  NOT NULL CHECK (type IN ('IMAGE', 'VIDEO', 'MODEL_3D')),
    title_en     VARCHAR(200) NOT NULL,
    title_es     VARCHAR(200) NOT NULL,
    title_fr     VARCHAR(200) NOT NULL,
    description_en TEXT,
    description_es TEXT,
    description_fr TEXT,
    published_at   TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at     TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CONSTRAINT media_item_description_all_or_none CHECK (
        (description_en IS NULL) = (description_es IS NULL)
            AND (description_en IS NULL) = (description_fr IS NULL))
);

CREATE TABLE media_technology
(
    media_item_id   INT          NOT NULL REFERENCES media_item (id) ON DELETE CASCADE,
    technology_type VARCHAR(100) NOT NULL REFERENCES technology (type),
    PRIMARY KEY (media_item_id, technology_type)
);

-- ER: NewsItem.
CREATE TABLE news_item
(
    id           SERIAL PRIMARY KEY,
    title_en     VARCHAR(200) NOT NULL,
    title_es     VARCHAR(200) NOT NULL,
    title_fr     VARCHAR(200) NOT NULL,
    text_en      TEXT         NOT NULL,
    text_es      TEXT         NOT NULL,
    text_fr      TEXT         NOT NULL,
    published_at TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at   TIMESTAMPTZ  NOT NULL DEFAULT now(),
    -- Id of the approved request. No foreign key: the request is deleted after approval.
    source_request_id INT UNIQUE
);

-- ER: MediaFile. Belongs to one media item OR one news item, never both.
CREATE TABLE media_file
(
    id                 SERIAL PRIMARY KEY,
    original_file_name VARCHAR(255) NOT NULL,
    stored_file_name   VARCHAR(255) NOT NULL UNIQUE,
    content_type       VARCHAR(100) NOT NULL,
    media_item_id      INT UNIQUE REFERENCES media_item (id) ON DELETE CASCADE,
    news_item_id       INT REFERENCES news_item (id) ON DELETE CASCADE,
    CONSTRAINT media_file_belongs_to_one CHECK ((media_item_id IS NULL) <> (news_item_id IS NULL))
);

CREATE TABLE country_deployment
(
    id                 SERIAL PRIMARY KEY,
    country_code       VARCHAR(2)    NOT NULL UNIQUE REFERENCES country (code),
    kilometers_of_mesh NUMERIC(8, 2) NOT NULL CHECK (kilometers_of_mesh >= 0),
    number_of_devices  INT           NOT NULL CHECK (number_of_devices >= 0),
    photo              VARCHAR(255),
    info_en            TEXT          NOT NULL,
    info_es            TEXT          NOT NULL,
    info_fr            TEXT          NOT NULL,
    published_at       TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at         TIMESTAMPTZ   NOT NULL DEFAULT now()
);

CREATE TABLE reference_adjustment
(
    id                 SERIAL PRIMARY KEY,
    kilometers_changed NUMERIC(8, 2) NOT NULL CHECK (kilometers_changed <> 0),
    published_at       TIMESTAMPTZ   NOT NULL DEFAULT now()
);

CREATE TABLE change_request
(
    id           SERIAL PRIMARY KEY,
    target_type  VARCHAR(30) NOT NULL CHECK (target_type IN
                                             ('NEWS', 'MEDIA', 'COUNTRY_DEPLOYMENT', 'REFERENCE_ADJUSTMENT')),
    type         VARCHAR(10) NOT NULL CHECK (type IN ('CREATE', 'UPDATE', 'DELETE')),
    target_id    INT,
    payload      JSONB,
    status       VARCHAR(10) NOT NULL DEFAULT 'PENDING' CHECK (status IN ('PENDING', 'RETURNED')),
    requested_by INT         NOT NULL REFERENCES app_user (id),
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    reviewed_by  INT REFERENCES app_user (id),
    -- a delete has no payload, create and update always have one
    CONSTRAINT change_request_payload_unless_delete CHECK ((type = 'DELETE') = (payload IS NULL)),
    -- update and delete must say which item they change
    CONSTRAINT change_request_target_unless_create CHECK (type = 'CREATE' OR target_id IS NOT NULL)
);

-- Only one open request per item.
CREATE UNIQUE INDEX change_request_one_open_per_item
    ON change_request (target_type, target_id)
    WHERE target_id IS NOT NULL;

-- ER: ReferenceTotal (derived).
-- ER: ReferenceTotal (derived, updates itself): total_km and years_of_experience.
CREATE VIEW reference_total AS
SELECT (SELECT COALESCE(SUM(kilometers_of_mesh), 0) FROM country_deployment)
           + (SELECT COALESCE(SUM(kilometers_changed), 0) FROM reference_adjustment) AS total_km,
       EXTRACT(YEAR FROM age(current_date, DATE '2007-06-28'))::INT         AS years_of_experience;