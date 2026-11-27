using System.Net;
using System.Text.Json;

namespace Dashboard.Services.Loading;

/// <summary>
/// Loads page data, ignores outdated results and cancels when disposed.
/// Each page owns its own instance.
/// </summary>
public sealed class PageLoader<T> : IDisposable where T : class
{
    private CancellationTokenSource? activeLoad;
    private bool disposed;

    public T? Data { get; private set; }

    public bool IsLoading { get; private set; }

    public string? ErrorMessage { get; private set; }

    public int Version { get; private set; }

    public async Task<bool> LoadAsync(
        Func<CancellationToken, Task<T>> load,
        string failureMessage)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        int version = ++Version;

        // A new load replaces the previous one.
        activeLoad?.Cancel();

        var cancellation = new CancellationTokenSource();
        activeLoad = cancellation;

        Data = null;
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            T result = await load(cancellation.Token);

            if (!IsCurrent(version, cancellation))
            {
                return false;
            }

            Data = result;
            return true;
        }
        catch (OperationCanceledException)
            when (cancellation.IsCancellationRequested)
        {
            // Navigation, disposal or another load cancelled this operation.
            return false;
        }
        catch (Exception exception) when (
            exception is HttpRequestException or
            JsonException or
            InvalidOperationException or
            OperationCanceledException or
            IOException)
        {
            if (IsCurrent(version, cancellation))
            {
                ErrorMessage = GetErrorMessage(exception, failureMessage);
            }

            return false;
        }
        finally
        {
            if (!disposed && version == Version)
            {
                IsLoading = false;
                activeLoad = null;
            }

            cancellation.Dispose();
        }
    }

    private bool IsCurrent(
        int version,
        CancellationTokenSource cancellation)
    {
        return !disposed &&
               version == Version &&
               !cancellation.IsCancellationRequested;
    }

    private static string GetErrorMessage(
        Exception exception,
        string failureMessage)
    {
        return exception switch
        {
            HttpRequestException
            {
                StatusCode: HttpStatusCode.NotFound
            } => "The requested item could not be found.",

            HttpRequestException
            {
                StatusCode: HttpStatusCode.Forbidden
            } => "You do not have permission to view this information.",

            OperationCanceledException =>
                "Loading took too long. Please try again.",

            _ => failureMessage
        };
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        ++Version;

        activeLoad?.Cancel();
        activeLoad = null;

        Data = null;
        IsLoading = false;
    }
}