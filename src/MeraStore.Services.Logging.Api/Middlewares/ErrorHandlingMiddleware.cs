using MeraStore.Services.Logging.Domain.Attributes;
using MeraStore.Shared.Kernel.Exceptions;
using MeraStore.Shared.Kernel.Exceptions.Core;
using MeraStore.Shared.Kernel.Exceptions.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Logging.Api.Middlewares;

/// <summary>
/// Global error-handling middleware to catch, transform, and log exceptions into structured problem details.
/// </summary>
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (BaseAppException ex)
        {
            await HandleStructuredExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            var wrapped = new CommonException(
                EventCodeRegistry.GetCode(Shared.Kernel.Exceptions.Constants.EventCodes.InternalServerError),
                ErrorCodeRegistry.GetCode(Shared.Kernel.Exceptions.Constants.ErrorCodes.InternalServerError),
                HttpStatusCode.InternalServerError,
                "An unexpected server error occurred.",
                innerException: ex?.InnerException,
                category: ExceptionCategory.Operational,
                severity: ExceptionSeverity.Critical
            );

            await HandleStructuredExceptionAsync(context, wrapped);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, FluentValidation.ValidationException exception)
    {
        logger.LogError(exception, "Validation error occurred: {@ValidationErrors}",
            exception.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var validationErrors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(validationErrors)
        {
            Type = GetRequestEventCode(context),
            Status = context.Response.StatusCode,
            Title = "Validation failed.",
            Instance = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, _jsonSerializerSettings));
    }

    private Task HandleStructuredExceptionAsync(HttpContext context, BaseAppException exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)exception.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Type = exception.EventCode ?? GetRequestEventCode(context),
            Title =  "An error occurred while processing your request.",
            Detail = exception.Message,
            Instance = context.TraceIdentifier
        };

        // Structured extensions for richer diagnostics
        problemDetails.Extensions["errorCode"] = exception.FullErrorCode;
        problemDetails.Extensions["category"] = exception.Category.ToString();
        problemDetails.Extensions["severity"] = exception.Severity.ToString();
        problemDetails.Extensions["service"] = ServiceCodeRegistry.GetKey(exception.ServiceIdentifier);
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        logger.LogError(exception, "Structured error occurred: {Message} | Code: {Code} | Category: {Category}",
            exception.Message, exception.FullErrorCode, exception.Category);

        return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, _jsonSerializerSettings));
    }

    /// <summary>
    /// Resolves the event code attached to the endpoint, if any.
    /// </summary>
    private string GetRequestEventCode(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var code = endpoint?.Metadata.GetMetadata<EventCodeAttribute>()?.EventCode;
        return string.IsNullOrWhiteSpace(code)
            ? Shared.Kernel.Exceptions.Constants.EventCodes.InternalServerError
            : code;
    }
}
