﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;
public class AdvertService : IAdvertService
{
    private readonly ILogger<AdvertService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public AdvertService(ILogger<AdvertService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public Task<AdvertDto> CreateAdvertAsync(AdvertCreateDto advertCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<AdvertDto> GetAdvertByIdAsync(int advertId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AdvertDto>> GetAllAdvertsAsync()
    {
        throw new NotImplementedException();
    }
}
