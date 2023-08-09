using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class ResetRolesModel
{
    public string UserId { get; set; }
    public List<string> RoleNames { get; set; }
}
