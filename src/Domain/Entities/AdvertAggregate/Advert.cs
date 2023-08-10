using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.FileAggregate;
//using static System.Net.Mime.MediaTypeNames;

namespace ArrayApp.Domain.Entities.AdvertAggregate;

public class Advert : BaseAuditableEntity, IAggregateRoot
{
    // The advert's title
    public string Title { get; set; }

    // The advert's description
    public string Description { get; set; }

    // The date and time the advert was created
    public DateTime CreatedAt { get; set; }

    // The date and time the advert was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the advert
    //public ApplicationUser Creator { get; set; }

    // The advert's price
    public decimal Price { get; set; }

    // The advert's location
    public string Location { get; set; }

    // A flag indicating whether the advert is active or not
    public bool IsActive { get; set; }

    // A list of images associated with the advert
    public List<FileData> Images { get; set; }

    // The advert's category
    public Category Category { get; set; }
}

//public class Category
//{
//}
//public class Image
//{
//}
//public class User
//{
//}
/*
This Advert class includes properties for storing information about the advert's title, description, creation and modification dates, creator, price, location, active status, associated images, and category. Of course, you can add or remove properties from this class based on the specific requirements of your application.
*/