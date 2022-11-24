using BusinessLogic.ExceptionMiddleware;

namespace RentApi.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserExceptionHandlerMiddleware>();
        }
    }
}