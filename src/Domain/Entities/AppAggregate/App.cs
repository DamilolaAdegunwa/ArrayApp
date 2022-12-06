using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.AppAggregate;
public class App
{
    // The app's name
    public string Name { get; set; }

    // The app's description
    public string Description { get; set; }

    // The date and time the app was created
    public DateTime CreatedAt { get; set; }

    // The date and time the app was last modified
    public DateTime ModifiedAt { get; set; }

    // The user who created the app
    public User Creator { get; set; }

    // The app's price
    public decimal Price { get; set; }

    // The app's average rating
    public double Rating { get; set; }

    // The app's category
    public Category Category { get; set; }

    // The app's version number
    public string Version { get; set; }

    // A list of screenshots associated with the app
    public List<Image> Screenshots { get; set; }

    // The app's release notes
    public string ReleaseNotes { get; set; }
}

/*
 This App class includes properties for storing information about the app's name, description, creation and modification dates, creator, price, average rating, category, version number, associated screenshots, and release notes. Of course, you can add or remove properties from this class based on the specific requirements of your application.
 */