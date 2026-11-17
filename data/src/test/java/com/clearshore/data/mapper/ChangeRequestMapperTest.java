package com.clearshore.data.mapper;

import com.clearshore.data.entity.ChangeType;
import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
import com.clearshore.data.generated.ChangeRequestProto;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ChangeRequestMapperTest {

    @Test
    void mapsACreateRequestWithPayloadToTheEntity() {
        ChangeRequestProto.ChangeRequest proto = ChangeRequestProto.ChangeRequest.newBuilder()
                .setTarget(ChangeRequestProto.ChangeTarget.CHANGE_TARGET_MEDIA)
                .setAction(ChangeRequestProto.ChangeAction.CHANGE_ACTION_CREATE)
                .setPayloadJson("{\"titleEn\":\"Boom\"}")
                .setStatus(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING)
                .setRequestedBy(7)
                .build();

        ChangeRequest request = ChangeRequestMapper.toEntity(proto);

        assertEquals(ChangeTarget.MEDIA, request.getTargetType());
        assertEquals(ChangeType.CREATE, request.getType());
        assertEquals(ChangeStatus.PENDING, request.getStatus());
        assertEquals("{\"titleEn\":\"Boom\"}", request.getPayload());
        assertEquals(7, request.getRequestedBy());
        assertNull(request.getTargetId());
    }

    @Test
    void mapsADeleteRequestWithoutPayloadToTheEntity() {
        ChangeRequestProto.ChangeRequest proto = ChangeRequestProto.ChangeRequest.newBuilder()
                .setTarget(ChangeRequestProto.ChangeTarget.CHANGE_TARGET_NEWS)
                .setAction(ChangeRequestProto.ChangeAction.CHANGE_ACTION_DELETE)
                .setTargetId(12)
                .setStatus(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING)
                .setRequestedBy(7)
                .build();

        ChangeRequest request = ChangeRequestMapper.toEntity(proto);

        assertEquals(12, request.getTargetId());
        assertNull(request.getPayload());
    }

    @Test
    void replyCarriesTheCreatedAtAndOmitsEmptyOptionals() {
        ChangeRequest request = new ChangeRequest(ChangeTarget.MEDIA, ChangeType.CREATE, null,
                "{}", 7);

        ChangeRequestProto.ChangeRequest proto = ChangeRequestMapper.toProto(request);

        assertEquals(ChangeRequestProto.ChangeTarget.CHANGE_TARGET_MEDIA, proto.getTarget());
        assertEquals(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING, proto.getStatus());
        assertEquals(request.getCreatedAt().getEpochSecond(), proto.getCreatedAt().getSeconds());
        assertFalse(proto.hasTargetId());
        assertEquals("{}", proto.getPayloadJson());
    }

    @Test
    void unspecifiedEnumsAreRejected() {
        ChangeRequestProto.ChangeRequest proto = ChangeRequestProto.ChangeRequest.getDefaultInstance();

        assertThrows(IllegalArgumentException.class, () -> ChangeRequestMapper.toEntity(proto));
    }
}
