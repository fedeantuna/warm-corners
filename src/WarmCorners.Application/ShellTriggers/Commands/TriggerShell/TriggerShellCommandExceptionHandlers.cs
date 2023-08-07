using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace WarmCorners.Application.ShellTriggers.Commands.TriggerShell;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TriggerShellCommandValidationExceptionHandler : IRequestExceptionHandler<TriggerShellCommand, Unit, ValidationException>
{
    internal const string RequestValidationExceptionLogMessageTemplate = "Request: {RequestName} {@Request} failed {@Errors}";

    private readonly ILogger<TriggerShellCommandValidationExceptionHandler> _logger;

    public TriggerShellCommandValidationExceptionHandler(ILogger<TriggerShellCommandValidationExceptionHandler> logger) =>
        this._logger = logger;

    public Task Handle(TriggerShellCommand request,
        ValidationException exception,
        RequestExceptionHandlerState<Unit> state,
        CancellationToken cancellationToken)
    {
        const string requestName = nameof(TriggerShellCommand);
        this._logger.LogError(RequestValidationExceptionLogMessageTemplate,
            requestName, request, exception.Errors);

        state.SetHandled(default);
        return Task.CompletedTask;
    }
}
