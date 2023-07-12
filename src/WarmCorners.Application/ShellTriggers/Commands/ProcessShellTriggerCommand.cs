using MediatR;
using Microsoft.Extensions.Logging;
using WarmCorners.Application.Common.Services;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.ShellTriggers.Commands;

public class ProcessShellTriggerCommand : IRequest
{
    public required ScreenCorner ScreenCorner { get; init; }
    public required string ShellCommand { get; init; }
    public required (short X, short Y) Position { get; init; }
}

public class ProcessShellTriggerCommandHandler : IRequestHandler<ProcessShellTriggerCommand>
{
    internal const string ExecutedShellCommandLogMessageTemplate = "Executed {ShellCommand}";

    private readonly ILogger<ProcessShellTriggerCommandHandler> _logger;
    private readonly IScreenService _screenService;
    private readonly IShellService _shellService;

    public ProcessShellTriggerCommandHandler(ILogger<ProcessShellTriggerCommandHandler> logger,
        IShellService shellService,
        IScreenService screenService)
    {
        this._logger = logger;
        this._shellService = shellService;
        this._screenService = screenService;
    }

    public async Task Handle(ProcessShellTriggerCommand request, CancellationToken cancellationToken)
    {
        var shouldTriggerCommand =
            this._screenService.IsMouseCursorInCorner(request.ScreenCorner, request.Position.X, request.Position.Y);

        if (!shouldTriggerCommand)
            return;

        await this._shellService.Run(request.ShellCommand);

        this._logger.LogInformation(ExecutedShellCommandLogMessageTemplate, request.ShellCommand);
    }
}
