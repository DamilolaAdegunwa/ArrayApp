using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(ILogger<ProductService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public Task<ProductDto> CreateProductAsync(ProductCreateDto productCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ProductDto> GetProductByIdAsync(int productId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        throw new NotImplementedException();
    }
}
