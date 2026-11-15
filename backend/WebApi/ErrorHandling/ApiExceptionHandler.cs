using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Services;

namespace WebApi.ErrorHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        int status = exception switch
        {
            ValidationFailedException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            DataAccessException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };
        string title = status == StatusCodes.Status500InternalServerError ? "Unexpected error" : exception.Message;

        ProblemDetails problem = new() { Status = status, Title = title };               
        if (exception is ValidationFailedException validation)                           
        {
            problem.Extensions["errors"] = validation.Errors;                            
        }
        
        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);    
        return true;
    }
}
