namespace WebPracticalTask.ProgramLogic
{
    public class RequestLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestLimiterService _limiterService;

        public RequestLimiterMiddleware(
            RequestDelegate next,
            RequestLimiterService limiterService)
        {
            _next = next;
            _limiterService = limiterService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_limiterService.TryAcquireSlot())
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "text/plain";
                var status = _limiterService.GetStatus();
                await context.Response.WriteAsync(
                    $"Service unavailable. Current requests: {status.current}/{status.limit}");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                _limiterService.ReleaseSlot();
            }
        }
    }
}
