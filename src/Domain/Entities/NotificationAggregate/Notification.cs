using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.NotificationAggregate;
public class Notification
{
    // The notification's title
    public string Title { get; set; }

    // The notification's body
    public string Body { get; set; }

    // The date and time the notification was created
    public DateTime CreatedAt { get; set; }

    // The user who the notification is for
    public User Recipient { get; set; }

    // The notification's type (e.g. "alert" or "reminder")
    public string Type { get; set; }

    // The notification's importance level (e.g. "high" or "low")
    public string Importance { get; set; }

    // The notification's status (e.g. "unread" or "read")
    public string Status { get; set; }

    // The notification's category (if it has one)
    public Category Category { get; set; }

    // The notification's tags (if it has any)
    public List<string> Tags { get; set; }
}