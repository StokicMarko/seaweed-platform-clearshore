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
}
