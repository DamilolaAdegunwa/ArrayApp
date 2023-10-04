using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class ClaimsIdentityDto
{
    public ClaimsIdentityActorDto? Actor { get; set; }
    public string? AuthenticationType { get; set; }
    public object? BootstrapContext { get; set; }
    public List<ClaimDto>? Claims { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Label { get; set; }
    public string? Name { get; set; }
    public string NameClaimType { get; set; }
    public string RoleClaimType { get; set; }
}

public class ClaimsIdentityActorDto
{
    public string? AuthenticationType { get; set; }
    public object? BootstrapContext { get; set; }
    public List<ClaimDto>? Claims { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Label { get; set; }
    public string? Name { get; set; }
    public string NameClaimType { get; set; }
    public string RoleClaimType { get; set; }
}
