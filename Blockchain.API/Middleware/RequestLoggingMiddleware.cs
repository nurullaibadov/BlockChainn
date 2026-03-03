namespace Blockchain.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            var start = DateTime.UtcNow;
            await _next(ctx);
            var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;

            _logger.LogInformation(
                "HTTP {Method} {Path} → {StatusCode} in {Elapsed:0}ms",
                ctx.Request.Method,
                ctx.Request.Path,
                ctx.Response.StatusCode,
                elapsed);
        }
    }
}
