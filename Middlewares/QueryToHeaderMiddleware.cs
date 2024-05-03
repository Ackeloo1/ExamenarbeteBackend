using Microsoft.Extensions.Primitives;

namespace TestMediatR1.Middlewares
{
    public class QueryToHeaderMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Query.TryGetValue("authorization", out StringValues values))
            {
                await next(context);
            }
            else
            {
                context.Request.Headers.Add("authorization", "Bearer " + values.ToString());
                await next(context);
            }
        }
    }
}
