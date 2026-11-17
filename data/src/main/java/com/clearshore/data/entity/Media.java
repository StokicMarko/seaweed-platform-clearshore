package com.clearshore.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.Table;
import jakarta.persistence.ManyToMany;
import jakarta.persistence.JoinTable;

import java.util.Set;
import java.util.Collections;
import java.util.LinkedHashSet;

import java.time.Instant;

@Entity
@Table(name = "media")
public class Media {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    @ManyToMany
    @JoinTable(name = "media_technology",
        joinColumns = @JoinColumn(name = "media_id"),
        inverseJoinColumns = @JoinColumn(name = "technology_type"))
    private Set<Technology> technologies = new LinkedHashSet<>();

    @Enumerated(EnumType.STRING)
    private MediaType mediaType;

    private String titleEn;
    private String titleEs;
    private String titleFr;

    private String descriptionEn;
    private String descriptionEs;
    private String descriptionFr;

    private String originalFileName;
    private String storedFileName;
    private String contentType;

    private Instant publishedAt;
    private Instant updatedAt;

    protected Media() {
    }

    public Media(Set<Technology> technologies, MediaType mediaType,
                 String titleEn, String titleEs, String titleFr,
                 String descriptionEn, String descriptionEs, String descriptionFr,
                 String originalFileName, String storedFileName, String contentType) {
        this.technologies = new LinkedHashSet<>(technologies);
        this.mediaType = mediaType;
        this.titleEn = titleEn;
        this.titleEs = titleEs;
        this.titleFr = titleFr;
        this.descriptionEn = descriptionEn;
        this.descriptionEs = descriptionEs;
        this.descriptionFr = descriptionFr;
        this.originalFileName = originalFileName;
        this.storedFileName = storedFileName;
        this.contentType = contentType;
        this.publishedAt = Instant.now();
        this.updatedAt = publishedAt;
    }

    public int getId() {
        return id;
    }

    public Set<Technology> getTechnologies() { return Collections.unmodifiableSet(technologies); }

    public MediaType getMediaType() {
        return mediaType;
    }

    public String getTitleEn() {
        return titleEn;
    }

    public String getTitleEs() {
        return titleEs;
    }

    public String getTitleFr() {
        return titleFr;
    }

    public String getDescriptionEn() {
        return descriptionEn;
    }

    public String getDescriptionEs() {
        return descriptionEs;
    }

    public String getDescriptionFr() {
        return descriptionFr;
    }

    public String getOriginalFileName() {
        return originalFileName;
    }

    public String getStoredFileName() {
        return storedFileName;
    }

    public String getContentType() {
        return contentType;
    }

    public Instant getPublishedAt() {
        return publishedAt;
    }

    public Instant getUpdatedAt() {
        return updatedAt;
    }
}
