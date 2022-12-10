using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities.FileAggregate;
public class DataFile : BaseAuditableEntity, IAggregateRoot
{
    // The file's name (including the file extension)
    public string Name { get; set; }

    // The file's size in bytes
    public long Size { get; set; }

    // The file's MIME type (e.g. "application/pdf")
    public string MimeType { get; set; }

    // The date and time the file was created
    public DateTime CreatedAt { get; set; }

    // The date and time the file was last modified
    public DateTime ModifiedAt { get; set; }

    // The file's path on the local file system
    public string Path { get; set; }

    // A flag indicating whether the file is read-only
    public bool IsReadOnly { get; set; }

    // The file's extension (e.g. ".pdf")
    public string Extension { get; set; }
}
/*
 This File class includes properties for storing information about the file's name, size, MIME type, creation and modification dates, path on the local file system, read-only status, and extension. Of course, you can add or remove properties from this class based on the specific requirements of your application.
 */