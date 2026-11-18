package com.clearshore.data.mapper;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
import com.clearshore.data.entity.ChangeType;
import com.clearshore.data.generated.ChangeRequestProto;

public final class ChangeRequestMapper {

    private ChangeRequestMapper() {
    }

    public static ChangeRequest toEntity(ChangeRequestProto.ChangeRequest proto) {
        return new ChangeRequest(
                toEntity(proto.getTarget()),
                toEntity(proto.getAction()),
                proto.hasTargetId() ? proto.getTargetId() : null,
                proto.hasPayloadJson() ? proto.getPayloadJson() : null,
                proto.getRequestedBy());
    }

    public static ChangeRequestProto.ChangeRequest toProto(ChangeRequest request) {
        ChangeRequestProto.ChangeRequest.Builder builder = ChangeRequestProto.ChangeRequest.newBuilder()
                .setId(request.getId())
                .setTarget(toProto(request.getTargetType()))
                .setAction(toProto(request.getType()))
                .setStatus(toProto(request.getStatus()))
                .setRequestedBy(request.getRequestedBy())
                .setCreatedAt(CommonMapper.toTimestamp(request.getCreatedAt()));

        if (request.getTargetId() != null) {
            builder.setTargetId(request.getTargetId());
        }
        if (request.getPayload() != null) {
            builder.setPayloadJson(request.getPayload());
        }
        return builder.build();
    }

    private static ChangeTarget toEntity(ChangeRequestProto.ChangeTarget target) {
        return switch (target) {
            case CHANGE_TARGET_NEWS -> ChangeTarget.NEWS;
            case CHANGE_TARGET_MEDIA -> ChangeTarget.MEDIA;
            case CHANGE_TARGET_COUNTRY_DEPLOYMENT -> ChangeTarget.COUNTRY_DEPLOYMENT;
            case CHANGE_TARGET_REFERENCE_ADJUSTMENT -> ChangeTarget.REFERENCE_ADJUSTMENT;
            default -> throw new IllegalArgumentException("Unknown change target: " + target);
        };
    }

    private static ChangeType toEntity(ChangeRequestProto.ChangeAction action) {
        return switch (action) {
            case CHANGE_ACTION_CREATE -> ChangeType.CREATE;
            case CHANGE_ACTION_UPDATE -> ChangeType.UPDATE;
            case CHANGE_ACTION_DELETE -> ChangeType.DELETE;
            default -> throw new IllegalArgumentException("Unknown change action: " + action);
        };
    }

    private static ChangeRequestProto.ChangeTarget toProto(ChangeTarget target) {
        return switch (target) {
            case NEWS -> ChangeRequestProto.ChangeTarget.CHANGE_TARGET_NEWS;
            case MEDIA -> ChangeRequestProto.ChangeTarget.CHANGE_TARGET_MEDIA;
            case COUNTRY_DEPLOYMENT -> ChangeRequestProto.ChangeTarget.CHANGE_TARGET_COUNTRY_DEPLOYMENT;
            case REFERENCE_ADJUSTMENT -> ChangeRequestProto.ChangeTarget.CHANGE_TARGET_REFERENCE_ADJUSTMENT;
        };
    }

    private static ChangeRequestProto.ChangeAction toProto(ChangeType type) {
        return switch (type) {
            case CREATE -> ChangeRequestProto.ChangeAction.CHANGE_ACTION_CREATE;
            case UPDATE -> ChangeRequestProto.ChangeAction.CHANGE_ACTION_UPDATE;
            case DELETE -> ChangeRequestProto.ChangeAction.CHANGE_ACTION_DELETE;
        };
    }

    private static ChangeRequestProto.ChangeStatus toProto(ChangeStatus status) {
        return switch (status) {
            case PENDING -> ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING;
            case RETURNED -> ChangeRequestProto.ChangeStatus.CHANGE_STATUS_RETURNED;
        };
    }
}