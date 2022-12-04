using MediatR;

namespace ArrayApp.Domain.Common;

public abstract class BaseEvent : INotification
{
    /// <summary>
    /// time the event occured (generic to all events)
    /// </summary>
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
