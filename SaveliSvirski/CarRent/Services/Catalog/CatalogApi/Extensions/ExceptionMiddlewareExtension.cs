using BusinessLogic.ExceptionMiddleware;

namespace CatalogApi.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserExceptionHandlerMiddleware>();
        }
    }
}