using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
//internal class LoginModel
//{
//}
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }
}