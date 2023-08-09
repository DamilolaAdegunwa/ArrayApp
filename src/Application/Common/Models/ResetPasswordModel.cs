using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class ResetPasswordModel
{
    public string UserId { get; set; }
    public string NewPassword { get; set; }
}
