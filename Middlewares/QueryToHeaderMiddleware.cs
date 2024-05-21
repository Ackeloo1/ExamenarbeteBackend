using Microsoft.Extensions.Primitives;

namespace TestMediatR1.Middlewares
{
    public class QueryToHeaderMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue("Upgrade", out StringValues upgradeValue) && upgradeValue != "websocket")
            {
                await next(context);
                return;
            }

            if (!context.Request.Query.TryGetValue("authorization", out StringValues authorizationValue))
            {
                context.Response.StatusCode = 400;
                return;
            }

            context.Request.Headers.Authorization = authorizationValue;

            await next(context);
            return;
        }
    }
}
