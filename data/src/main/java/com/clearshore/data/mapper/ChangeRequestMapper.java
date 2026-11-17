package com.clearshore.data.mapper;

import com.clearshore.data.entity.ChangeAction;
import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
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
                toEntity(proto.getStatus()),
                proto.getRequestedBy());
    }

    public static ChangeRequestProto.ChangeRequest toProto(ChangeRequest request) {
        ChangeRequestProto.ChangeRequest.Builder builder = ChangeRequestProto.ChangeRequest.newBuilder()
                .setId(request.getId())
                .setTarget(toProto(request.getTarget()))
                .setAction(toProto(request.getAction()))
                .setStatus(toProto(request.getStatus()))
                .setRequestedBy(request.getRequestedBy())
                .setCreatedAt(CommonMapper.toTimestamp(request.getCreatedAt()));

        if (request.getTargetId() != null) {
            builder.setTargetId(request.getTargetId());
        }
        if (request.getPayloadJson() != null) {
            builder.setPayloadJson(request.getPayloadJson());
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

    private static ChangeAction toEntity(ChangeRequestProto.ChangeAction action) {
        return switch (action) {
            case CHANGE_ACTION_CREATE -> ChangeAction.CREATE;
            case CHANGE_ACTION_UPDATE -> ChangeAction.UPDATE;
            case CHANGE_ACTION_DELETE -> ChangeAction.DELETE;
            default -> throw new IllegalArgumentException("Unknown change action: " + action);
        };
    }

    private static ChangeStatus toEntity(ChangeRequestProto.ChangeStatus status) {
        return switch (status) {
            case CHANGE_STATUS_PENDING -> ChangeStatus.PENDING;
            case CHANGE_STATUS_RETURNED -> ChangeStatus.RETURNED;
            default -> throw new IllegalArgumentException("Unknown change status: " + status);
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

    private static ChangeRequestProto.ChangeAction toProto(ChangeAction action) {
        return switch (action) {
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
