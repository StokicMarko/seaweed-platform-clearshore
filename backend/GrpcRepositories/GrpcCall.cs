using Grpc.Core;
using RepositoryContracts;

namespace GrpcRepositories;

internal static class GrpcCall
{
    public static async Task<T> RunAsync<T>(Func<Task<T>> call)
    {
        try
        {
            return await call();
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            throw new NotFoundException(e.Status.Detail);
        }
        catch (RpcException e)
        {
            throw new DataAccessException($"The data server failed: {e.Status.Detail}", e);
        }
    }

    public static async Task RunAsync(Func<Task> call)
    {
        await RunAsync(async () =>
        {
            await call();
            return true;
        });
    }
}
