using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Domain.Entities;
[Table("Permissions")]
public class Permission
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}

[Table("UserPermissions")]
public class UserPermission
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }
}
//dotnet ef migrations add init_userperm --context Auth.Exercise.Two.Persistence.AppDbContext
//dotnet ef database update --context Auth.Exercise.Two.Persistence.AppDbContext
