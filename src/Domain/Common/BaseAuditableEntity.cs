using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArrayApp.Domain.Common;

/// <summary>
/// All the basic properties that every entity should need
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    [Key]
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    public virtual int Id { get; set; }

    public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

    public virtual int CreatorUserId { get; set; }

    public virtual int? DeleterUserId { get; set; }

    public virtual DateTimeOffset? DeletionTime { get; set; }

    public virtual bool IsDeleted { get; set; }

    public virtual DateTimeOffset? LastModificationTime { get; set; }

    public virtual int LastModifierUserId { get; set; }

    public BaseAuditableEntity() 
    {
        CreatorUserId = 0;
        DeleterUserId = null;
        DeletionTime = null;
        IsDeleted = false;
        LastModificationTime = null;
        LastModifierUserId = 0;
    }
}
