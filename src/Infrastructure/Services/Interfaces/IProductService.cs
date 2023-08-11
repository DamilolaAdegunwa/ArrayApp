using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IProductService
{
    Task<ProductDto> CreateProductAsync(ProductCreateDto productCreateDto);
    Task<ProductDto> GetProductByIdAsync(int productId);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);

}
