using Entities;

namespace GrpcRepositories;

internal static class MediaMapper
{
    //receive
    public static Media ToEntity(Generated.Media media) => new()
    {
        Id = media.Id,
       Technologies = media.Technologies.Select(TechnologyMapper.ToEntity).ToList(),
        MediaType = ToEntity(media.MediaType),
        Title = CommonMapper.ToEntity(media.Title),
        Description = media.Description is null ? null : CommonMapper.ToEntity(media.Description),
        OriginalFileName = media.OriginalFileName,
        StoredFileName = media.StoredFileName,
        ContentType = media.ContentType,
        PublishedAt = media.PublishedAt.ToDateTimeOffset(),
        UpdatedAt = media.UpdatedAt.ToDateTimeOffset()
    };
    
    
    // Create = all four file fields must have a value. (entity to grpc), send
    //applier checks this, thorw ?? should not fire, but stops bugs from saving media item with a missing file
    public static Generated.CreateMediaRequest ToCreateRequest(MediaSubmission submission)
    {
        Generated.CreateMediaRequest request = new()
        {
            MediaType = ToProto(submission.MediaType
                                ?? throw new ArgumentException("A create needs a media type.", nameof(submission))),
            Title = CommonMapper.ToProto(submission.Title),
            OriginalFileName = submission.OriginalFileName
                               ?? throw new ArgumentException("A create needs the original file name.", nameof(submission)),
            StoredFileName = submission.StoredFileName
                             ?? throw new ArgumentException("A create needs the stored file name.", nameof(submission)),
            ContentType = submission.ContentType
                          ?? throw new ArgumentException("A create needs the content type.", nameof(submission))
        };

        // list made (genereted/ .proto)(class owned)
        //                              Add items to the list(our own list)
        request.TechnologyTypes.AddRange(submission.TechnologyTypes);

        // Not set means no description.
        if (submission.Description is not null)
        {
            request.Description = CommonMapper.ToProto(submission.Description);
        }

        return request;
    }

    // (Entity to gRPC request), file fields - only set when they have a value.
    public static Generated.UpdateMediaRequest ToUpdateRequest(int id, MediaSubmission submission)
    {
        Generated.UpdateMediaRequest request = new()
        {
            Id = id,
            Title = CommonMapper.ToProto(submission.Title)
        };

        request.TechnologyTypes.AddRange(submission.TechnologyTypes);

        if (submission.Description is not null)
        {
            request.Description = CommonMapper.ToProto(submission.Description); //in localizedtext, needs convertion, no .value (class can be null)
        }

        // Not set means keep the current file.
        if (submission.MediaType is not null)
        {
            request.MediaType = ToProto(submission.MediaType.Value); //enun=/ generated enu so ToProto()..., nullable enum = wrapper = needs .value
        }

        if (submission.OriginalFileName is not null)
        {
            request.OriginalFileName = submission.OriginalFileName; //plain string, same ype, no conversion, straigh copy
        }

        if (submission.StoredFileName is not null)
        {
            request.StoredFileName = submission.StoredFileName;
        }

        if (submission.ContentType is not null)
        {
            request.ContentType = submission.ContentType;
        }

        return request;
    }

    

    private static MediaType ToEntity(Generated.MediaType mediaType) => mediaType switch
    {
        Generated.MediaType.Image => MediaType.Image,
        Generated.MediaType.Video => MediaType.Video,
        Generated.MediaType.Model3D => MediaType.Model3D,
        _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, "Unknown media type")
    };
    
    
    // (Entity enum to gRPC enum) They are two different types with the same value names.
                    //returns                 //takes in
    private static Generated.MediaType ToProto(MediaType mediaType) => mediaType switch
    {
        // switch
        MediaType.Image => Generated.MediaType.Image,
        MediaType.Video => Generated.MediaType.Video,
        MediaType.Model3D => Generated.MediaType.Model3D,

        // _ = "anything else". Runs if a new enum value is added and not mapped here.
                         // throw -> so the mistake instead of sending a wrong type to the data server.
        _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, "Unknown media type")
    };
    
}
