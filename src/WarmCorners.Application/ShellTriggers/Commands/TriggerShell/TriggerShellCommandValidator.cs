using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace WarmCorners.Application.ShellTriggers.Commands.TriggerShell;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class TriggerShellCommandValidator : AbstractValidator<TriggerShellCommand>
{
    public TriggerShellCommandValidator() =>
        this.RuleFor(tsc => tsc.ShellCommand)
            .NotEmpty();
}
