using System.Diagnostics.CodeAnalysis;
using WarmCorners.Application.Common.Wrappers;

namespace WarmCorners.Infrastructure.Wrappers;

[ExcludeFromCodeCoverage]
public class DateTimeOffsetWrapper : IDateTimeOffsetWrapper
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
