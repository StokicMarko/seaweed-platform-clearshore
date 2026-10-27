package com.clearshore.data.mapper;

import com.clearshore.data.grpc.MediaItem;
import com.clearshore.data.grpc.MediaMetadata;
import com.clearshore.data.model.Media;

public final class MediaMapper {

    private MediaMapper() {}

    public static MediaItem toProto(Media m) {
        return MediaItem.newBuilder()
                .setId(m.id())
                .setTitleEn(nz(m.titleEn()))
                .setTitleFr(nz(m.titleFr()))
                .setTitleEs(nz(m.titleEs()))
                .setDescriptionEn(nz(m.descriptionEn()))
                .setDescriptionFr(nz(m.descriptionFr()))
                .setDescriptionEs(nz(m.descriptionEs()))
                .setFileName(nz(m.fileName()))
                .setUrl(nz(m.url()))
                .setContentType(nz(m.contentType()))
                .setCreatedAt(m.createdAt() == null ? "" : m.createdAt().toString())
                .setMachine(nz(m.machine()))
                .build();
    }

    // id = 0, url = null, createdAt = null: they are filled in later by the service/DB
    public static Media toDomain(MediaMetadata md) {
        return new Media(0,
                md.getTitleEn(), md.getTitleFr(), md.getTitleEs(),
                emptyToNull(md.getDescriptionEn()), emptyToNull(md.getDescriptionFr()),
                emptyToNull(md.getDescriptionEs()),
                md.getFileName(), null, md.getContentType(), null,
                emptyToNull(md.getMachine()));
    }

    private static String nz(String s) { return s == null ? "" : s; }

    private static String emptyToNull(String s) { return s == null || s.isEmpty() ? null : s; }
}
