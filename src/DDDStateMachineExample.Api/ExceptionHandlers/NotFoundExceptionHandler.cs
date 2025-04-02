using DDDStateMachineExample.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DDDStateMachineExample.Api.ExceptionHandlers;

public partial class NotFoundExceptionHandler(
    ILogger<NotFoundExceptionHandler> logger
    )
    : IExceptionHandler
{
    private const string ProblemTitle = "Not Found";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (!TryGetException(exception, out var notFoundException))
            return false;

        LogExceptionOccurred(logger, notFoundException, notFoundException.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = ProblemTitle,
            Detail = notFoundException.Message,
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
                         .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static bool TryGetException(
        Exception exception,
        out Exception targetEx
    )
    {
        if (exception is NotFoundException nfe)
        {
            targetEx = nfe;
            return true;
        }

        targetEx = new Exception();
        return false;
    }

    [LoggerMessage(EventId = 0001, Level = LogLevel.Error, Message = "Exception occurred: {Message}")]
    private static partial void LogExceptionOccurred(ILogger logger, Exception? exception, string message);
}
