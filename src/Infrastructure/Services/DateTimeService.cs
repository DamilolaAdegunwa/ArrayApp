using ArrayApp.Application.Common.Interfaces;

namespace ArrayApp.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTime Now => DateTime.UtcNow;
}
