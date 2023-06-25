using System.Reactive.Concurrency;

namespace WarmCorners.Core.Wrappers;

public interface ISchedulerWrapper
{
    IScheduler Default { get; }
}

public class SchedulerWrapper : ISchedulerWrapper
{
    public IScheduler Default => Scheduler.Default;
}
