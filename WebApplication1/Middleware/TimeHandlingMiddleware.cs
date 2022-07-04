using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApplication1.Middleware
{
    public class TimeHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<TimeHandlingMiddleware> _logger;
        private Stopwatch stop;
        public TimeHandlingMiddleware(ILogger<TimeHandlingMiddleware> logger)
        {
            _logger = logger;
            stop = new Stopwatch();
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            stop.Start();
            await next.Invoke(context);
            stop.Stop();
            var mili = stop.ElapsedMilliseconds;
            if (mili / 1000 > 4)
            {
                var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {mili} ms";
                _logger.LogInformation(message);
            }

        }
    }
}
