using Entities;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories;

public class MediaGrpcRepository(Generated.MediaData.MediaDataClient client) : IMediaRepository
{
    public Task<List<Media>> GetManyAsync(string? technologyType) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.GetManyMediaRequest request = new();
            if (technologyType is not null)
            {
                request.TechnologyType = technologyType;
            }

            Generated.MediaList reply = await client.GetManyAsync(request);
            return reply.Items.Select(MediaMapper.ToEntity).ToList();
        });

    public Task<Media> GetSingleAsync(int id) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.Media reply = await client.GetSingleAsync(new Generated.MediaId { Id = id });
            return MediaMapper.ToEntity(reply);
        });

    public Task CopyFileToAsync(int id, Stream destination, CancellationToken cancellationToken) =>
        GrpcCall.RunAsync(async () =>
        {
            using AsyncServerStreamingCall<Generated.FileChunk> call =
                client.ReadFile(new Generated.MediaId { Id = id }, cancellationToken: cancellationToken);

            await foreach (Generated.FileChunk chunk in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                await destination.WriteAsync(chunk.Data.Memory, cancellationToken);
            }
        });

    //Tier 2 asks data sever = new item row
    public Task<Media> CreateAsync(MediaSubmission submission) =>
        GrpcCall.RunAsync(async () => //runAsync runs the call and translates gRPC errors into pur exceptions
        {
            //mapper turns entity  (mediaSUubmission) into gRPC request message, client.CreateAsync send to the JAVA server and waits 
            Generated.Media reply = await client.CreateAsync(MediaMapper.ToCreateRequest(submission));
            
            // mapper turns the gRPC reply back into entity, so tier 2 never sees gRPC types.
            return MediaMapper.ToEntity(reply);
        });

    //same as create but with which media item (id) to replace
    public Task<Media> UpdateAsync(int id, MediaSubmission submission) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.Media reply = await client.UpdateAsync(MediaMapper.ToUpdateRequest(id, submission));
            return MediaMapper.ToEntity(reply); //media item after update = reply
        });

    public Task DeleteAsync(int id) =>
        GrpcCall.RunAsync(async () =>
        {
            await client.DeleteAsync(new Generated.MediaId { Id = id });
        });
}