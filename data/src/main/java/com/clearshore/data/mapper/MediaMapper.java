package com.clearshore.data.mapper;

import com.clearshore.data.entity.MediaItem;
import com.clearshore.data.entity.MediaType;
import com.clearshore.data.generated.MediaProto;
import com.clearshore.data.entity.Technology;
import java.util.Comparator;
import java.util.List;

public final class MediaMapper {

    private MediaMapper() {
    }

    public static MediaProto.Media toProto(MediaItem mediaItem) {
        MediaProto.Media.Builder builder = MediaProto.Media.newBuilder()
                .setId(mediaItem.getId())
                .setMediaType(toProto(mediaItem.getMediaType()))
                .setTitle(CommonMapper.toLocalizedText(mediaItem.getTitleEn(), mediaItem.getTitleEs(), mediaItem.getTitleFr()))
                .setOriginalFileName(mediaItem.getOriginalFileName())
                .setContentType(mediaItem.getContentType())
                .setPublishedAt(CommonMapper.toTimestamp(mediaItem.getPublishedAt()))
                .setUpdatedAt(CommonMapper.toTimestamp(mediaItem.getUpdatedAt()));

        mediaItem.getTechnologies().stream()
            .sorted(Comparator.comparing(Technology::getType))
            .forEach(t -> builder.addTechnologies(TechnologyMapper.toProto(t)));
        if (mediaItem.getDescriptionEn() != null) {
            builder.setDescription(CommonMapper.toLocalizedText(
                    mediaItem.getDescriptionEn(), mediaItem.getDescriptionEs(), mediaItem.getDescriptionFr()));
        }
        return builder.build();
    }

    public static MediaProto.MediaList toProtoList(List<MediaItem> mediaItems) {
        return MediaProto.MediaList.newBuilder()
                .addAllItems(mediaItems.stream().map(MediaMapper::toProto).toList())
                .build();
    }

    private static MediaProto.MediaType toProto(MediaType mediaType) {
        return switch (mediaType) {
            case IMAGE -> MediaProto.MediaType.MEDIA_TYPE_IMAGE;
            case VIDEO -> MediaProto.MediaType.MEDIA_TYPE_VIDEO;
            case MODEL_3D -> MediaProto.MediaType.MEDIA_TYPE_MODEL_3D;
        };
    }
}
