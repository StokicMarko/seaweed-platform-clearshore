using Google.Protobuf;
using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories;

/// Saves and deletes files on the Java data server over gRPC.
public class FileGrpcRepository(Generated.FileData.FileDataClient client) : IFileRepository
{
    // gRPC messages are limited to 4 MB, so a file is sent as many small chunks (research based)
    private const int ChunkSize = 64 * 1024;

    public Task<string> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken) =>
        GrpcCall.RunAsync(async () =>
        {
            using AsyncClientStreamingCall<Generated.SaveFileRequest, Generated.StoredFile> call =
                client.Save(cancellationToken: cancellationToken);

            // First message: the original file name.
            await call.RequestStream.WriteAsync(
                new Generated.SaveFileRequest { OriginalFileName = originalFileName }, cancellationToken);

            // Every following message: one chunk of the file, read straight from the stream.
            byte[] buffer = new byte[ChunkSize];
            int read;
            while ((read = await content.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await call.RequestStream.WriteAsync(
                    new Generated.SaveFileRequest { Chunk = ByteString.CopyFrom(buffer, 0, read) },
                    cancellationToken);
            }

            await call.RequestStream.CompleteAsync();

            // The data server answers with the unique name it gave the file.
            Generated.StoredFile reply = await call.ResponseAsync;
            return reply.StoredFileName;
        });

    public Task DeleteAsync(string storedFileName) =>
        GrpcCall.RunAsync(async () =>
        {
            await client.DeleteAsync(new Generated.StoredFile { StoredFileName = storedFileName });
        });

    public Task CopyToAsync(string storedFileName, Stream destination, CancellationToken cancellationToken) =>
        GrpcCall.RunAsync(async () =>
        {
            using AsyncServerStreamingCall<Generated.FileChunk> call =
                client.Read(new Generated.StoredFile { StoredFileName = storedFileName },
                    cancellationToken: cancellationToken);

            await foreach (Generated.FileChunk chunk in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                await destination.WriteAsync(chunk.Data.Memory, cancellationToken);
            }
        });
}
