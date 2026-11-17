package com.clearshore.data.mapper;

import com.clearshore.data.entity.Technology;
import com.clearshore.data.entity.MediaItem;
import com.clearshore.data.entity.MediaType;
import com.clearshore.data.generated.MediaProto;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;

import java.util.Set;

class MediaItemMapperTest {

    @Test
    void mapsMediaWithTechnologyAndDescription() {
        Technology technology = new Technology("mesh-boom-hd");
        MediaItem mediaItem = new MediaItem(Set.of(technology), MediaType.VIDEO,
                "Boom in action", "Barrera en acción", "Barrage en action",
                "Installed in 2025", "Instalada en 2025", "Installé en 2025",
                "boom.mp4", "3f2a-boom.mp4", "video/mp4");

        MediaProto.Media proto = MediaMapper.toProto(mediaItem);

        assertEquals(MediaProto.MediaType.MEDIA_TYPE_VIDEO, proto.getMediaType());
        assertEquals("Barrera en acción", proto.getTitle().getEs());
        assertEquals("Installé en 2025", proto.getDescription().getFr());
        assertEquals("mesh-boom-hd", proto.getTechnologies(0).getType());
        assertEquals("boom.mp4", proto.getOriginalFileName());
        assertEquals(mediaItem.getPublishedAt().getEpochSecond(), proto.getPublishedAt().getSeconds());
    }

    @Test
    void leavesTechnologyAndDescriptionEmptyWhenMissing() {
        MediaItem mediaItem = new MediaItem(Set.of(), MediaType.IMAGE,
                "Beach", "Playa", "Plage",
                null, null, null,
                "beach.jpg", "9c1d-beach.jpg", "image/jpeg");

        MediaProto.Media proto = MediaMapper.toProto(mediaItem);

        assertEquals(0, proto.getTechnologiesCount());
        assertFalse(proto.hasDescription());
    }
}
