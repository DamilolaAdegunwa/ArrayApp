using System;
using System.ComponentModel.DataAnnotations.Schema;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities;

public class Product : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

[Table("Tests", Schema = "csharp")]
public class Test : BaseAuditableEntity, IAggregateRoot
{
    public string MyTestProp { get; set; } = string.Empty;
    public string MyTestProp2 { get; set; } = string.Empty;
}
