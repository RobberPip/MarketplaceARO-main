using Microsoft.AspNetCore.Diagnostics;
using Shared;

namespace App.ServiceExtensions {
    public class CustomExceptionHandler : IExceptionHandler {
        private readonly ILogger<CustomExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetails = new ErrorResponse {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred",
                Detail = "",
            };
            if (_environment.IsDevelopment())
                problemDetails.Detail = exception.Message;


            _logger.LogError(exception, "An error {@ErrorType} occured with details: {@Details}", exception.GetType(), exception.Message);
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
