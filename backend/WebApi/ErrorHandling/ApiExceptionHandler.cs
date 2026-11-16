using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.ErrorHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        int status = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            DataAccessException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };
        string title = status == StatusCodes.Status500InternalServerError ? "Unexpected error" : exception.Message;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title },
            cancellationToken);
        return true;
    }
}
