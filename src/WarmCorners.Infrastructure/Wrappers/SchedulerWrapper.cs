using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using WarmCorners.Application.Common.Wrappers;

namespace WarmCorners.Infrastructure.Wrappers;

[ExcludeFromCodeCoverage]
public class SchedulerWrapper : ISchedulerWrapper
{
    public IScheduler Default => Scheduler.Default;
}
