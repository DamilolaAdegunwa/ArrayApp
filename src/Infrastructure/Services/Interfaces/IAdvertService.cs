using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IAdvertService
{
    Task<AdvertDto> CreateAdvertAsync(AdvertCreateDto advertCreateDto);
    Task<AdvertDto> GetAdvertByIdAsync(int advertId);
    Task<IEnumerable<AdvertDto>> GetAllAdvertsAsync();
}
