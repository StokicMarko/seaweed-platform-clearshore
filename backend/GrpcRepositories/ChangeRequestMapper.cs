using Entities;

namespace GrpcRepositories;

/// Converts between the .NET ChangeRequest and the generated gRPC message.
internal static class ChangeRequestMapper
{
    public static Generated.ChangeRequest ToProto(ChangeRequest request)
    {
        Generated.ChangeRequest proto = new()
        {
            TargetType = ToProto(request.TargetType),
            Type = ToProto(request.Type),
            Status = ToProto(request.Status),
            RequestedBy = request.RequestedBy
        };

        // Optional are only set when they have a value target empty for create, payloadjson empy in delete (because nothing new is proposed)
        if (request.TargetId is not null)
        {
            proto.TargetId = request.TargetId.Value;
        }

        if (request.PayloadJson is not null)
        {
            proto.PayloadJson = request.PayloadJson;
        }

        return proto;
    }

    public static ChangeRequest ToEntity(Generated.ChangeRequest proto) => new()
    {
        Id = proto.Id,
        TargetType = ToEntity(proto.TargetType),
        Type = ToEntity(proto.Type),
        TargetId = proto.HasTargetId ? proto.TargetId : null,
        PayloadJson = proto.HasPayloadJson ? proto.PayloadJson : null,
        Status = ToEntity(proto.Status),
        RequestedBy = proto.RequestedBy,
        CreatedAt = proto.CreatedAt.ToDateTimeOffset()
    };

    
    public static Generated.GetManyChangeRequestsRequest ToGetManyRequest(
        ChangeTarget? targetType,
        int? targetId,
        ChangeStatus? status)
    {
        Generated.GetManyChangeRequestsRequest proto = new();

        if (targetType is not null)
        {
            proto.TargetType = ToProto(targetType.Value);
        }

        if (targetId is not null)
        {
            proto.TargetId = targetId.Value;
        }

        if (status is not null)
        {
            proto.Status = ToProto(status.Value);
        }

        return proto;
    }
    
    //req of UpdateStatus, which request, and its new status.
    public static Generated.UpdateChangeRequestStatusRequest ToUpdateStatusRequest(int id, ChangeStatus status) => new()
    {
        Id = id,
        Status = ToProto(status)
    };

    private static Generated.ChangeTarget ToProto(ChangeTarget target) => target switch
    {
        ChangeTarget.News => Generated.ChangeTarget.News,
        ChangeTarget.Media => Generated.ChangeTarget.Media,
        ChangeTarget.CountryDeployment => Generated.ChangeTarget.CountryDeployment,
        ChangeTarget.ReferenceAdjustment => Generated.ChangeTarget.ReferenceAdjustment,
        _ => throw new ArgumentOutOfRangeException(nameof(target), target, "Unknown change target")
    };

    private static ChangeTarget ToEntity(Generated.ChangeTarget target) => target switch
    {
        Generated.ChangeTarget.News => ChangeTarget.News,
        Generated.ChangeTarget.Media => ChangeTarget.Media,
        Generated.ChangeTarget.CountryDeployment => ChangeTarget.CountryDeployment,
        Generated.ChangeTarget.ReferenceAdjustment => ChangeTarget.ReferenceAdjustment,
        _ => throw new ArgumentOutOfRangeException(nameof(target), target, "Unknown change target")
    };

    private static Generated.ChangeType ToProto(ChangeType type) => type switch
    {
        ChangeType.Create => Generated.ChangeType.Create,
        ChangeType.Update => Generated.ChangeType.Update,
        ChangeType.Delete => Generated.ChangeType.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown change type")
    };

    private static ChangeType ToEntity(Generated.ChangeType action) => action switch
    {
        Generated.ChangeType.Create => ChangeType.Create,
        Generated.ChangeType.Update => ChangeType.Update,
        Generated.ChangeType.Delete => ChangeType.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown change type")
    };

    private static Generated.ChangeStatus ToProto(ChangeStatus status) => status switch
    {
        ChangeStatus.Pending => Generated.ChangeStatus.Pending,
        ChangeStatus.Returned => Generated.ChangeStatus.Returned,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown change status")
    };

    private static ChangeStatus ToEntity(Generated.ChangeStatus status) => status switch
    {
        Generated.ChangeStatus.Pending => ChangeStatus.Pending,
        Generated.ChangeStatus.Returned => ChangeStatus.Returned,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown change status")
    };
}