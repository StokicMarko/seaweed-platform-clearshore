package com.clearshore.data.mapper;

import com.clearshore.data.entity.Media;
import com.clearshore.data.entity.MediaType;
import com.clearshore.data.generated.MediaProto;

import java.util.List;

public final class MediaMapper {

    private MediaMapper() {
    }

    public static MediaProto.Media toProto(Media media) {
        MediaProto.Media.Builder builder = MediaProto.Media.newBuilder()
                .setId(media.getId())
                .setMediaType(toProto(media.getMediaType()))
                .setTitle(CommonMapper.toLocalizedText(media.getTitleEn(), media.getTitleEs(), media.getTitleFr()))
                .setOriginalFileName(media.getOriginalFileName())
                .setContentType(media.getContentType())
                .setPublishedAt(CommonMapper.toTimestamp(media.getPublishedAt()))
                .setUpdatedAt(CommonMapper.toTimestamp(media.getUpdatedAt()));

        if (media.getMachine() != null) {
            builder.setMachine(MachineMapper.toProto(media.getMachine()));
        }
        if (media.getDescriptionEn() != null) {
            builder.setDescription(CommonMapper.toLocalizedText(
                    media.getDescriptionEn(), media.getDescriptionEs(), media.getDescriptionFr()));
        }
        return builder.build();
    }

    public static MediaProto.MediaList toProtoList(List<Media> media) {
        return MediaProto.MediaList.newBuilder()
                .addAllItems(media.stream().map(MediaMapper::toProto).toList())
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
