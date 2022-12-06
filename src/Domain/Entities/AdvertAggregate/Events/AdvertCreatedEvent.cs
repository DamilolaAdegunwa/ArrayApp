using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Domain.Entities.AdvertAggregate.Events;
internal class AdvertCreatedEvent
{
}
/*

 AdvertCreatedNotificationHandler: This event could be raised when a new advert is created. It could include information about the advert, such as its title and description.
AdvertUpdatedNotificationHandler: This event could be raised when an existing advert is updated. It could include information about the changes that were made to the advert.
AdvertDeletedNotificationHandler: This event could be raised when an existing advert is deleted. It could include information about the advert that was deleted.
AdvertViewedNotificationHandler: This event could be raised when an existing advert is viewed by a user. It could include information about the user who viewed the advert and the date and time when it was viewed.
AdvertPurchasedNotificationHandler: This event could be raised when an existing advert is purchased by a user. It could include information about the user who purchased the advert, the price of the advert, and the date and time of the purchase.
AdvertExpiredNotificationHandler: This event could be raised when an existing advert's active period has expired. It could include information about the advert that has expired.
Of course, you can add or remove events from this list based on the specific requirements of your application.

 */