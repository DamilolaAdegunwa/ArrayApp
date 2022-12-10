using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ArrayApp.Domain.Entities;

public class ApplicationUser : IdentityUser
{

    [Key]
    public string Id { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccountConfirmationCode { get; set; }
    public UserType UserType { get; set; }
}
public static class UserExtensions
{

    public static bool IsNull(this ApplicationUser user)
    {
        return user == null;
    }

    public static bool IsConfirmed(this ApplicationUser user)
    {
        return user.EmailConfirmed || user.PhoneNumberConfirmed;
    }

    public static bool AccountLocked(this ApplicationUser user)
    {
        return user.LockoutEnabled == true;
    }

    public static bool HasNoPassword(this ApplicationUser user)
    {
        return string.IsNullOrWhiteSpace(user.PasswordHash);
    }
}