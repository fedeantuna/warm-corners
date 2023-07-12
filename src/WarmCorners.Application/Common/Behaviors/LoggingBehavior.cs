using System.Diagnostics.CodeAnalysis;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace WarmCorners.Application.Common.Behaviors;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    internal const string LogMessageTemplate = "Request: {RequestName} {@Request}";

    private readonly ILogger _logger;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
    [SuppressMessage("ReSharper", "ContextualLoggerProblem")]
    public LoggingBehavior(ILogger<TRequest> logger) =>
        this._logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        this._logger.LogInformation(LogMessageTemplate,
            requestName, request);

        return Task.CompletedTask;
    }
}
