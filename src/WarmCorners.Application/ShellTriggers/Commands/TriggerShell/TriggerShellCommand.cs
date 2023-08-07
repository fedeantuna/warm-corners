using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using WarmCorners.Application.Common.Wrappers;

namespace WarmCorners.Application.ShellTriggers.Commands.TriggerShell;

public class TriggerShellCommand : IRequest
{
    public required string ShellCommand { get; init; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TriggerShellCommandHandler : IRequestHandler<TriggerShellCommand>
{
    internal const string ExecutedShellCommandLogMessageTemplate = "Executed {ShellCommand}";
    internal const string ErrorExecutingShellCommandLogMessageTemplate = "Error executing {ShellCommand}";

    private readonly ILogger<TriggerShellCommandHandler> _logger;
    private readonly IProcessWrapper _processWrapper;

    public TriggerShellCommandHandler(ILogger<TriggerShellCommandHandler> logger,
        IProcessWrapper processWrapper)
    {
        this._logger = logger;
        this._processWrapper = processWrapper;
    }

    public Task Handle(TriggerShellCommand request, CancellationToken cancellationToken)
    {
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
