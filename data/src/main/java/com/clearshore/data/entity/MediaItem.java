package com.clearshore.data.entity;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.JoinTable;
import jakarta.persistence.ManyToMany;
import jakarta.persistence.OneToOne;
import jakarta.persistence.Table;

import java.time.Instant;
import java.util.Collections;
import java.util.LinkedHashSet;
import java.util.Set;


@Entity
@Table(name = "media_item")
public class MediaItem {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;


    @ManyToMany
    @JoinTable(name = "media_technology",
            joinColumns = @JoinColumn(name = "media_item_id"),
            inverseJoinColumns = @JoinColumn(name = "technology_type"))
    private Set<Technology> technologies = new LinkedHashSet<>();


    @Enumerated(EnumType.STRING)
    @Column(name = "type")
    private MediaType mediaType;

    private String titleEn;
    private String titleEs;
    private String titleFr;


    @Column(name = "text_en")
    private String descriptionEn;
    @Column(name = "text_es")
    private String descriptionEs;
    @Column(name = "text_fr")
    private String descriptionFr;


    @OneToOne(mappedBy = "mediaItem", cascade = CascadeType.ALL, orphanRemoval = true)
    private MediaFile file;

    private Instant publishedAt;
    private Instant updatedAt;


    protected MediaItem() {
    }


    public MediaItem(Set<Technology> technologies, MediaType mediaType,
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
        this.file = new MediaFile(originalFileName, storedFileName, contentType, this);
        this.publishedAt = Instant.now();
        this.updatedAt = publishedAt;
    }

    public int getId() {
        return id;
    }

    public Set<Technology> getTechnologies() {
        return Collections.unmodifiableSet(technologies);
    }

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

    // The three file getters read from the MediaFile, so callers did not have to change.
    public String getOriginalFileName() {
        return file.getOriginalFileName();
    }

    public String getStoredFileName() {
        return file.getStoredFileName();
    }

    public String getContentType() {
        return file.getContentType();
    }

    public Instant getPublishedAt() {
        return publishedAt;
    }

    public Instant getUpdatedAt() {
        return updatedAt;
    }
}