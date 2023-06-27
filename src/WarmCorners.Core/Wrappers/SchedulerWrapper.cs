using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;

namespace WarmCorners.Core.Wrappers;

public interface ISchedulerWrapper
{
    IScheduler Default { get; }
}

[ExcludeFromCodeCoverage(Justification = "Wrappers are just lightweight abstractions to facilitate testing")]
public class SchedulerWrapper : ISchedulerWrapper
{
    public IScheduler Default => Scheduler.Default;
}
