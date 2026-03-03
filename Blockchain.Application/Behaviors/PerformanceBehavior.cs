using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer = new();

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            _timer.Restart();
            var response = await next();
            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
                _logger.LogWarning("Slow request: {Name} took {Elapsed}ms", typeof(TRequest).Name, _timer.ElapsedMilliseconds);

            return response;
        }
    }
}
