using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class TriggerKeyCombinationCommandValidator : AbstractValidator<TriggerKeyCombinationCommand>
{
    public TriggerKeyCombinationCommandValidator() =>
        this.RuleFor(tkc => tkc.KeyCombination)
            .NotEmpty();
}
