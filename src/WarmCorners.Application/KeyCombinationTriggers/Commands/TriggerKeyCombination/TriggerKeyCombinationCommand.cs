using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;

namespace WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;

public class TriggerKeyCombinationCommand : IRequest
{
    public required IReadOnlyCollection<KeyCode> KeyCombination { get; init; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TriggerKeyCombinationCommandHandler : IRequestHandler<TriggerKeyCombinationCommand>
{
    internal const string ExecutedKeyCombinationLogMessageTemplate = "Executed {KeyCombination}";

    private readonly IEventSimulator _eventSimulator;
    private readonly ILogger<TriggerKeyCombinationCommandHandler> _logger;

    public TriggerKeyCombinationCommandHandler(IEventSimulator eventSimulator,
        ILogger<TriggerKeyCombinationCommandHandler> logger)
    {
        this._eventSimulator = eventSimulator;
        this._logger = logger;
    }

    public Task Handle(TriggerKeyCombinationCommand request, CancellationToken cancellationToken)
    {
        request.KeyCombination.ToList().ForEach(key => this._eventSimulator.SimulateKeyPress(key));
        request.KeyCombination.ToList().ForEach(key => this._eventSimulator.SimulateKeyRelease(key));

        this._logger.LogInformation(ExecutedKeyCombinationLogMessageTemplate,
            string.Join('+', request.KeyCombination.Select(kc => kc.ToString())));

        return Task.CompletedTask;
    }
}
