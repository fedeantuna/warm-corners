using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.ShellTriggers.Commands;

public class ProcessShellTriggerCommand : IRequest
{
    public required ScreenCorner ScreenCorner { get; init; }
    public required string ShellCommand { get; init; }
    public required (short X, short Y) Position { get; init; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ProcessShellTriggerCommandHandler : IRequestHandler<ProcessShellTriggerCommand>
{
    internal const string ExecutedShellCommandLogMessageTemplate = "Executed {ShellCommand}";
    internal const string ErrorExecutingShellCommandLogMessageTemplate = "Error executing {ShellCommand}";

    private readonly ILogger<ProcessShellTriggerCommandHandler> _logger;
    private readonly IProcessWrapper _processWrapper;
    private readonly IScreenService _screenService;

    public ProcessShellTriggerCommandHandler(ILogger<ProcessShellTriggerCommandHandler> logger,
        IProcessWrapper processWrapper,
        IScreenService screenService)
    {
        this._logger = logger;
        this._processWrapper = processWrapper;
        this._screenService = screenService;
    }

    public Task Handle(ProcessShellTriggerCommand request, CancellationToken cancellationToken)
    {
        var shouldTriggerCommand =
            this._screenService.IsMouseCursorInCorner(request.ScreenCorner, request.Position.X, request.Position.Y);

        if (!shouldTriggerCommand)
            return Task.CompletedTask;

        this._processWrapper.SetStartInfo(new ProcessStartInfo("cmd", $"/c {request.ShellCommand}")
        {
            CreateNoWindow = true
        });
        var commandExecutedSuccessfully = this._processWrapper.Start();

        if (commandExecutedSuccessfully)
            this._logger.LogInformation(ExecutedShellCommandLogMessageTemplate, request.ShellCommand);
        else
            this._logger.LogError(ErrorExecutingShellCommandLogMessageTemplate, request.ShellCommand);

        return Task.CompletedTask;
    }
}
