using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
[Serializable]
public class JwtConfig
{
    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    /// <summary>
    /// Seconds 
    /// </summary>
    public int TokenDurationInSeconds { get; set; }
}
