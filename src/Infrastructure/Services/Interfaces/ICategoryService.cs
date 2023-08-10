using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface ICategoryService
{
    Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
    Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryUpdateDto);
    Task DeleteCategoryAsync(int categoryId);
    Task<IEnumerable<CategoryDto>> GetCategoriesByKeywordAsync(string keyword);
    Task<bool> IsCategoryNameUniqueAsync(string categoryName);
    Task<int> GetTotalCategoryCountAsync();
    Task<IEnumerable<CategoryDto>> GetTopCategoriesAsync(int count);
    Task<IEnumerable<CategoryDto>> GetPagedCategoriesAsync(int pageNumber, int pageSize);
}
