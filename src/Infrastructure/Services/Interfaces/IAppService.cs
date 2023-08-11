using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IAppService
{
    Task<AppDto> CreateAppAsync(AppCreateDto appCreateDto);
    Task<AppDto> GetAppByIdAsync(int appId);
    Task<IEnumerable<AppDto>> GetAllAppsAsync();
}
