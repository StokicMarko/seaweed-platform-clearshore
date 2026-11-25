package com.clearshore.data.mapper;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
import com.clearshore.data.entity.ChangeType;
import com.clearshore.data.generated.ChangeRequestProto;

import java.util.List;

public final class ChangeRequestMapper {

    private ChangeRequestMapper() {
    }

    public static ChangeRequest toEntity(ChangeRequestProto.ChangeRequest proto) {
        return new ChangeRequest(
                toEntity(proto.getTargetType()),
                toEntity(proto.getType()),
                proto.hasTargetId() ? proto.getTargetId() : null,
                proto.hasPayloadJson() ? proto.getPayloadJson() : null,
                proto.getRequestedBy());
    }

    public static ChangeRequestProto.ChangeRequest toProto(ChangeRequest request) {
        ChangeRequestProto.ChangeRequest.Builder builder = ChangeRequestProto.ChangeRequest.newBuilder()
                .setId(request.getId())
                .setTargetType(toProto(request.getTargetType()))
                .setType(toProto(request.getType()))
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

    public static ChangeTarget toEntity(ChangeRequestProto.ChangeTarget target) {
        return switch (target) {
            case CHANGE_TARGET_NEWS -> ChangeTarget.NEWS;
            case CHANGE_TARGET_MEDIA -> ChangeTarget.MEDIA;
            case CHANGE_TARGET_COUNTRY_DEPLOYMENT -> ChangeTarget.COUNTRY_DEPLOYMENT;
            case CHANGE_TARGET_REFERENCE_ADJUSTMENT -> ChangeTarget.REFERENCE_ADJUSTMENT;
            default -> throw new IllegalArgumentException("Unknown change target: " + target);
        };
    }

    public static ChangeStatus toEntity(ChangeRequestProto.ChangeStatus status) {
        return switch (status) {
            case CHANGE_STATUS_PENDING -> ChangeStatus.PENDING;
            case CHANGE_STATUS_RETURNED -> ChangeStatus.RETURNED;
            default -> throw new IllegalArgumentException("Unknown change status: " + status);
        };
    }

    public static ChangeRequestProto.ChangeRequestList toProtoList(List<ChangeRequest> requests) {
        return ChangeRequestProto.ChangeRequestList.newBuilder()
            .addAllItems(requests.stream().map(ChangeRequestMapper::toProto).toList())
            .build();
    }

    private static ChangeType toEntity(ChangeRequestProto.ChangeType type) {
        return switch (type) {
            case CHANGE_TYPE_CREATE -> ChangeType.CREATE;
            case CHANGE_TYPE_UPDATE -> ChangeType.UPDATE;
            case CHANGE_TYPE_DELETE -> ChangeType.DELETE;
            default -> throw new IllegalArgumentException("Unknown change type: " + type);
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

    private static ChangeRequestProto.ChangeType toProto(ChangeType type) {
        return switch (type) {
            case CREATE -> ChangeRequestProto.ChangeType.CHANGE_TYPE_CREATE;
            case UPDATE -> ChangeRequestProto.ChangeType.CHANGE_TYPE_UPDATE;
            case DELETE -> ChangeRequestProto.ChangeType.CHANGE_TYPE_DELETE;
        };
    }

    private static ChangeRequestProto.ChangeStatus toProto(ChangeStatus status) {
        return switch (status) {
            case PENDING -> ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING;
            case RETURNED -> ChangeRequestProto.ChangeStatus.CHANGE_STATUS_RETURNED;
        };
    }
}