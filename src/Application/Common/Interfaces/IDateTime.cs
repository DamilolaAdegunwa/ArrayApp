namespace ArrayApp.Application.Common.Interfaces;

public interface IDateTime
{
    DateTimeOffset UtcNow { get; }
    DateTime Now { get; }
}
