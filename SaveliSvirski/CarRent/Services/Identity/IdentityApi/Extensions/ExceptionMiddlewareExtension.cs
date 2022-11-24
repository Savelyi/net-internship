using BusinessLogic.ExceptionMiddleware;

namespace IdentityApi.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserExceptionHandlerMiddleware>();
        }
    }
}