using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Application.Common.Services;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.KeyCombinationTriggers.Commands;

public class ProcessKeyCombinationTriggerCommand : IRequest
{
    public required ScreenCorner ScreenCorner { get; init; }
    public required IReadOnlyCollection<KeyCode> KeyCombination { get; init; }
    public required (short X, short Y) Position { get; init; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ProcessKeyCombinationTriggerCommandHandler : IRequestHandler<ProcessKeyCombinationTriggerCommand>
{
    internal const string ExecutedKeyCombinationLogMessageTemplate = "Executed {KeyCombination}";

    private readonly IEventSimulator _eventSimulator;
    private readonly ILogger<ProcessKeyCombinationTriggerCommandHandler> _logger;
    private readonly IScreenService _screenService;

    public ProcessKeyCombinationTriggerCommandHandler(IEventSimulator eventSimulator,
        ILogger<ProcessKeyCombinationTriggerCommandHandler> logger,
        IScreenService screenService)
    {
        this._eventSimulator = eventSimulator;
        this._logger = logger;
        this._screenService = screenService;
    }

    public Task Handle(ProcessKeyCombinationTriggerCommand request, CancellationToken cancellationToken)
    {
        var shouldTriggerCommand =
            this._screenService.IsMouseCursorInCorner(request.ScreenCorner, request.Position.X, request.Position.Y);

        if (!shouldTriggerCommand || !request.KeyCombination.Any())
            return Task.CompletedTask;

        request.KeyCombination.ToList().ForEach(key => this._eventSimulator.SimulateKeyPress(key));
        request.KeyCombination.ToList().ForEach(key => this._eventSimulator.SimulateKeyRelease(key));

        this._logger.LogInformation(ExecutedKeyCombinationLogMessageTemplate,
            string.Join('+', request.KeyCombination.Select(kc => kc.ToString())));

        return Task.CompletedTask;
    }
}
