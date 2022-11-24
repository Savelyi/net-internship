using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedModels.ErrorModels;

namespace BusinessLogic.ExceptionMiddleware
{
    public class UserExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<UserExceptionHandlerMiddleware> _logger;

        public UserExceptionHandlerMiddleware(RequestDelegate next, ILogger<UserExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
            }
        }

        private Task HandleExceptionMessageAsync(HttpContext context, Exception ex)
        {
            var statusCode = GetStatusCode(ex);
            _logger.LogError($"Something went wrong: {ex.Message}, StatusCode: {statusCode}, TargetSite: {ex.TargetSite}");
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new ErrorDetails
            {
                StatusCode = statusCode,
                Message = ex.Message
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(result);
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                OperationCanceledException => 499,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}