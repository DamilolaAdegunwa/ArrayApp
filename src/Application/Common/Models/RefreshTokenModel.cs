using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class RefreshTokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
