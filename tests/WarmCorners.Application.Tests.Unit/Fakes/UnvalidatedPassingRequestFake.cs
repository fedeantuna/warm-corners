using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace WarmCorners.Application.Tests.Unit.Fakes;

public class UnvalidatedPassingRequestFake : IRequest
{
}

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class UnvalidatedPassingRequestFakeHandler : IRequestHandler<UnvalidatedPassingRequestFake>
{
    public Task Handle(UnvalidatedPassingRequestFake request, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
