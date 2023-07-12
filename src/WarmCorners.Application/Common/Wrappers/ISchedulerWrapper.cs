using System.Reactive.Concurrency;

namespace WarmCorners.Application.Common.Wrappers;

public interface ISchedulerWrapper
{
    IScheduler Default { get; }
}
