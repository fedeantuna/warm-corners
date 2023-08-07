using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class TriggerKeyCombinationCommandValidationExceptionHandler : IRequestExceptionHandler<TriggerKeyCombinationCommand, Unit,
    ValidationException>
{
    internal const string RequestValidationExceptionLogMessageTemplate = "Request: {RequestName} {@Request} failed {@Errors}";

    private readonly ILogger<TriggerKeyCombinationCommandValidationExceptionHandler> _logger;

    public TriggerKeyCombinationCommandValidationExceptionHandler(ILogger<TriggerKeyCombinationCommandValidationExceptionHandler> logger) =>
        this._logger = logger;

    public Task Handle(TriggerKeyCombinationCommand request,
        ValidationException exception,
        RequestExceptionHandlerState<Unit> state,
        CancellationToken cancellationToken)
    {
        const string requestName = nameof(TriggerKeyCombinationCommand);
        this._logger.LogError(RequestValidationExceptionLogMessageTemplate,
            requestName, request, exception.Errors);

        state.SetHandled(default);
        return Task.CompletedTask;
    }
}
