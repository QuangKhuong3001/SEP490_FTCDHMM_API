using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
                _logger.LogWarning("AppException caught: {Status} - {Message}", ex.ResponseCode.StatusCode, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.ResponseCode.StatusCode;

                var response = new
                {
                    code = ex.ResponseCode.Code,
                    statusCode = ex.ResponseCode.StatusCode,
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
                    code = AppResponseCode.UNKNOWN_ERROR.Code,
                    statusCode = AppResponseCode.UNKNOWN_ERROR.StatusCode,
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
