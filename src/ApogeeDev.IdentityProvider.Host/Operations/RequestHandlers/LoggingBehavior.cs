using System.Diagnostics;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        _logger.LogInformation("Handling {@Request}", typeof(TRequest).Name);
        var response = await next();
        stopwatch.Stop();
        _logger.LogInformation("Handled {@Response} {@EllapsedMsec}",
            typeof(TResponse).Name, stopwatch.ElapsedMilliseconds);

        return response;
    }
}