using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using ArrayApp.Domain.Entities;
namespace ArrayApp.Infrastructure.Identity;
public static class ClaimsExtensions
{
    public static List<Claim> UserToClaims(this ApplicationUser user)
    {
        //These wont be null
        var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
            };
        return claims;
    }
}
