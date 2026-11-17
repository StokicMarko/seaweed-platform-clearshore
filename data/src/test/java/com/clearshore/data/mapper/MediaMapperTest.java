package com.clearshore.data.mapper;

import com.clearshore.data.entity.Technology;
import com.clearshore.data.entity.Media;
import com.clearshore.data.entity.MediaType;
import com.clearshore.data.generated.MediaProto;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;

class MediaMapperTest {

    @Test
    void mapsMediaWithMachineAndDescription() {
        Technology technology = new Technology("mesh-boom-hd");
        Media media = new Media(technology, MediaType.VIDEO,
                "Boom in action", "Barrera en acción", "Barrage en action",
                "Installed in 2025", "Instalada en 2025", "Installé en 2025",
                "boom.mp4", "3f2a-boom.mp4", "video/mp4");

        MediaProto.Media proto = MediaMapper.toProto(media);

        assertEquals(MediaProto.MediaType.MEDIA_TYPE_VIDEO, proto.getMediaType());
        assertEquals("Barrera en acción", proto.getTitle().getEs());
        assertEquals("Installé en 2025", proto.getDescription().getFr());
        assertEquals("mesh-boom-hd", proto.getTechnology().getType());
        assertEquals("boom.mp4", proto.getOriginalFileName());
        assertEquals(media.getPublishedAt().getEpochSecond(), proto.getPublishedAt().getSeconds());
    }

    @Test
    void leavesMachineAndDescriptionEmptyWhenMissing() {
        Media media = new Media(null, MediaType.IMAGE,
                "Beach", "Playa", "Plage",
                null, null, null,
                "beach.jpg", "9c1d-beach.jpg", "image/jpeg");

        MediaProto.Media proto = MediaMapper.toProto(media);

        assertFalse(proto.hasTechnology());
        assertFalse(proto.hasDescription());
    }
}
