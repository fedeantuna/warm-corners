namespace WarmCorners.Application.Common.Wrappers;

public interface IDateTimeOffsetWrapper
{
    DateTimeOffset UtcNow { get; }
}
