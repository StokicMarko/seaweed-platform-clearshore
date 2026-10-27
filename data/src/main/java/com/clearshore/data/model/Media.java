package com.clearshore.data.model;

import java.time.Instant;

    public record Media(
            int id,
            String titleEn, String titleFr, String titleEs,
            String descriptionEn, String descriptionFr, String descriptionEs,
            String fileName, String url, String contentType,
            Instant createdAt, String machine) {

        public Media withId(int newId) {
            return new Media(newId, titleEn, titleFr, titleEs, descriptionEn, descriptionFr,
                    descriptionEs, fileName, url, contentType, createdAt, machine);
        }

        public Media withUrl(String newUrl) {
            return new Media(id, titleEn, titleFr, titleEs, descriptionEn, descriptionFr,
                    descriptionEs, fileName, newUrl, contentType, createdAt, machine);
        }
    }

