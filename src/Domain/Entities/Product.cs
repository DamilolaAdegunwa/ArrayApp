using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities;
public class Product : BaseAuditableEntity, IAggregateRoot
{
}

[Table("Tests", Schema = "csharp")]
public class Test : BaseAuditableEntity, IAggregateRoot
{
    public string MyTestProp { get; set; }
    public string MyTestProp2 { get; set; }
}
