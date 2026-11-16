using Entities;

namespace GrpcRepositories;

/// Converts between the .NET ChangeRequest and the generated gRPC message.
internal static class ChangeRequestMapper
{
    public static Generated.ChangeRequest ToProto(ChangeRequest request)
    {
        Generated.ChangeRequest proto = new()
        {
            Target = ToProto(request.Target),
            Action = ToProto(request.Action),
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
        Target = ToEntity(proto.Target),
        Action = ToEntity(proto.Action),
        TargetId = proto.HasTargetId ? proto.TargetId : null,
        PayloadJson = proto.HasPayloadJson ? proto.PayloadJson : null,
        Status = ToEntity(proto.Status),
        RequestedBy = proto.RequestedBy,
        CreatedAt = proto.CreatedAt.ToDateTimeOffset()
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

    private static Generated.ChangeAction ToProto(ChangeAction action) => action switch
    {
        ChangeAction.Create => Generated.ChangeAction.Create,
        ChangeAction.Update => Generated.ChangeAction.Update,
        ChangeAction.Delete => Generated.ChangeAction.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown change action")
    };

    private static ChangeAction ToEntity(Generated.ChangeAction action) => action switch
    {
        Generated.ChangeAction.Create => ChangeAction.Create,
        Generated.ChangeAction.Update => ChangeAction.Update,
        Generated.ChangeAction.Delete => ChangeAction.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown change action")
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