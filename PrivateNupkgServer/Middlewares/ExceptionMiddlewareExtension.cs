namespace privatenupkgserver.Middlewares;

public static class ExceptionMiddlewareExtension
{
    public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(c => c.Run(async httpContext =>
        {
            var exception = httpContext.Features.Get<IExceptionHandlerPathFeature>().Error;
            object resp = new { Message = $"'{exception.Message}', traceIdentifier: {httpContext.TraceIdentifier}" };
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(resp));
        }));
    }
}
