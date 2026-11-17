package com.clearshore.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToOne;
import jakarta.persistence.Table;

 //only the media item side is mapped here for now, till new entities
@Entity
@Table(name = "media_file")
public class MediaFile {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    private String originalFileName;
    private String storedFileName;
    private String contentType;

    // Which media item this file belongs to (column media_item_id).
    @OneToOne
    @JoinColumn(name = "media_item_id")
    private MediaItem mediaItem;


    protected MediaFile() {
    }

    // Package-private: only MediaItem creates its own file.
    MediaFile(String originalFileName, String storedFileName, String contentType, MediaItem mediaItem) {
        this.originalFileName = originalFileName;
        this.storedFileName = storedFileName;
        this.contentType = contentType;
        this.mediaItem = mediaItem;
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
}