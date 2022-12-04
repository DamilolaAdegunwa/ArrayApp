using ArrayApp.Application.Common.Interfaces;

namespace ArrayApp.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
