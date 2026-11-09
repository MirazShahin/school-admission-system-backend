using School.Domain.Exceptions;

namespace School.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (DomainException ex)
        {
            await Write(ctx, StatusFor(ex), ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled error");
            await Write(ctx, 500, "INTERNAL_ERROR", "Something went wrong. Please try again.");
        }
    }

    private static int StatusFor(DomainException ex) => ex switch
    {
        NotFoundException => 404,
        ValidationFailedException => 400,
        _ => 409
    };

    private static Task Write(HttpContext ctx, int status, string code, string message)
    {
        ctx.Response.StatusCode = status;
        return ctx.Response.WriteAsJsonAsync(new { code, message });
    }
}