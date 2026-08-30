using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities;
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

    public async Task<ProductDto> CreateProductAsync(ProductCreateDto productCreateDto)
    {
        _logger.LogInformation("Creating product: {Name}", productCreateDto.Name);
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            CategoryId = productCreateDto.CategoryId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.ProductBaseRepository.AddAsync(product);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.ProductBaseRepository.ListAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> GetProductByIdAsync(int productId)
    {
        var product = await _unitOfWork.ProductBaseRepository.GetByIdAsync(productId);
        return product != null ? MapToDto(product) : new ProductDto();
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.ProductBaseRepository.ListAsync();
        return products.Where(p => p.CategoryId == categoryId).Select(MapToDto);
    }

    private static ProductDto MapToDto(Product p) => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        CategoryId = p.CategoryId,
        CreatedAt = p.CreatedAt
    };
}
