using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning("AppException caught: {Status} - {Message}", ex.ResponseCode.Status, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.ResponseCode.Status;

                var response = new
                {
                    statusCode = ex.ResponseCode.Status,
                    code = ex.ResponseCode.Code,
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    statusCode = 500,
                    code = "INTERNAL SERVER ERROR",
                    message = "Internal Server Error"
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
