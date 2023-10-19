using System.Security.Claims;

using ArrayApp.Application.Common.Interfaces;

namespace ArrayApp.WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId"));
}