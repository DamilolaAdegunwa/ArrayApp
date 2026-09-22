using System;

namespace ArrayApp.Application.Common.Models;

public class ResetPasswordModel
{
    public string UserId { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
    public string? CurrentPassword { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
