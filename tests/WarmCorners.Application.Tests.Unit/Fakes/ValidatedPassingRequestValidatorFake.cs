﻿using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace WarmCorners.Application.Tests.Unit.Fakes;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ValidatedPassingRequestValidatorFake : AbstractValidator<ValidatedPassingRequestFake>
{
    public ValidatedPassingRequestValidatorFake() =>
        this.RuleFor(fe => fe.SomeString)
            .NotEmpty();
}
