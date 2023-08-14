using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Application.Common.Models;
using System.Data.Entity;
namespace ArrayApp.Infrastructure.Services;
public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ILogger<CategoryService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
    {
        // Guard clauses
        if (categoryCreateDto == null)
        {
            throw new ArgumentNullException(nameof(categoryCreateDto));
        }

        // Additional validation checks
        if (string.IsNullOrEmpty(categoryCreateDto.Name))
        {
            throw new ArgumentException("Category name must not be empty", nameof(categoryCreateDto.Name));
        }

        // Additional validation checks
        if (string.IsNullOrEmpty(categoryCreateDto.Description))
        {
            throw new ArgumentException("Category Description must not be empty", nameof(categoryCreateDto.Description));
        }

        categoryCreateDto.Name = categoryCreateDto.Name?.Trim();
        categoryCreateDto.Description = categoryCreateDto.Description?.Trim();
        var exist = _unitOfWork.CategoryBaseRepository.DbContextSet.FirstOrDefault(c => c.Name.ToLower() == categoryCreateDto.Name.ToLower());
        if (exist != null) 
        {
            throw new Exception("the category already exist!");
        }

        // Business logic for creating a category
        var newCategory = new Category
        {
            Name = categoryCreateDto.Name,
            Description = categoryCreateDto.Description
        };

        await _unitOfWork.CategoryBaseRepository.AddAsync(newCategory);
        //await _unitOfWork.CategoryBaseRepository.SaveChangesAsync();

        return new CategoryDto
        {
            Id = newCategory.Id,
            Name = newCategory.Name,
            Description = newCategory.Description
        };
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
    {
        // Guard clauses
        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categoryId), "Category ID must be greater than zero");
        }

        // Business logic for fetching a category
        var category = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new Exception($"Category with ID {categoryId} not found");
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        // Business logic for fetching all categories
        var categories = await _unitOfWork.CategoryBaseRepository.ListAsync();

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }
    public async Task UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryUpdateDto)
    {
        // Guard clauses
        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categoryId), "Category ID must be greater than zero");
        }
        if (categoryUpdateDto == null)
        {
            throw new ArgumentNullException(nameof(categoryUpdateDto));
        }

        // Fetch the existing category
        var existingCategory = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(categoryId);
        if (existingCategory == null)
        {
            throw new Exception($"Category with ID {categoryId} not found");
        }

        // Update properties
        existingCategory.Name = categoryUpdateDto.Name ?? existingCategory.Name;
        existingCategory.Description = categoryUpdateDto.Description ?? existingCategory.Description;

        // Save changes
        await _unitOfWork.CategoryBaseRepository.UpdateAsync(existingCategory);
        await _unitOfWork.CategoryBaseRepository.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        // Guard clauses
        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categoryId), "Category ID must be greater than zero");
        }

        // Fetch the existing category
        var existingCategory = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(categoryId);
        if (existingCategory == null)
        {
            throw new Exception($"Category with ID {categoryId} not found");
        }

        // Perform delete logic
        await _unitOfWork.CategoryBaseRepository.DeleteAsync(existingCategory);
        await _unitOfWork.CategoryBaseRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByKeywordAsync(string keyword)
    {
        // Implementation for getting categories by keyword
        var categories = _unitOfWork.CategoryBaseRepository.DbContextSet.Where(
            category => category.Name.Contains(keyword) || category.Description.Contains(keyword));

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    public async Task<bool> IsCategoryNameUniqueAsync(string categoryName)
    {
        // Implementation for checking if category name is unique
        var existingCategory = await _unitOfWork.CategoryBaseRepository.DbContextSet.FirstOrDefaultAsync(category => category.Name == categoryName);

        return existingCategory == null;
    }

    public async Task<int> GetTotalCategoryCountAsync()
    {
        // Implementation for getting total category count
        return await _unitOfWork.CategoryBaseRepository.CountAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetTopCategoriesAsync(int count)
    {
        // Implementation for getting top N categories
        var categories = (await _unitOfWork.CategoryBaseRepository.ListAsync()).Take(count);

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    //public async Task<CategoryStatsDto> GetCategoryStatisticsAsync(int categoryId)
    //{
    //    // Implementation for getting category statistics
    //    var category = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(categoryId);
    //    if (category == null)
    //    {
    //        throw new Exception($"Category with ID {categoryId} not found");
    //    }

    //    // Calculate statistics and return
    //    var statsDto = new CategoryStatsDto
    //    {
    //        CategoryId = category.Id,
    //        TotalProducts = category.Products.Count(), // Assuming there's a Products navigation property
    //        TotalSubcategories = category.Subcategories.Count(), // Assuming there's a Subcategories navigation property
    //        // Add more statistics as needed
    //    };

    //    return statsDto;
    //}

    //public async Task<IEnumerable<CategoryDto>> GetCategoriesByFilterAsync(CategoryFilterDto filterDto)
    //{
    //    // Implementation for getting categories by complex filtering criteria
    //    var filteredCategories = await _unitOfWork.CategoryBaseRepository.FindByAsync(category =>
    //        (filterDto.Name == null || category.Name.Contains(filterDto.Name)) &&
    //        (filterDto.MinTotalProducts == null || category.Products.Count() >= filterDto.MinTotalProducts) &&
    //        // Add more filtering conditions as needed
    //    );

    //    return filteredCategories.Select(category => new CategoryDto
    //    {
    //        Id = category.Id,
    //        Name = category.Name,
    //        Description = category.Description
    //    });
    //}

    public async Task<IEnumerable<CategoryDto>> GetPagedCategoriesAsync(int pageNumber, int pageSize)
    {
        // Implementation for getting paged categories
        var categories = (await _unitOfWork.CategoryBaseRepository.ListAsync()).Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    //public async Task<CategoryDto> MoveCategoryAsync(int categoryId, int newParentCategoryId)
    //{
    //    // Implementation for moving a category to a new parent category
    //    var category = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(categoryId);
    //    if (category == null)
    //    {
    //        throw new Exception($"Category with ID {categoryId} not found");
    //    }

    //    var newParentCategory = await _unitOfWork.CategoryBaseRepository.GetByIdAsync(newParentCategoryId);
    //    if (newParentCategory == null)
    //    {
    //        throw new Exception($"New parent category with ID {newParentCategoryId} not found");
    //    }

    //    category.ParentCategory = newParentCategory;

    //    _unitOfWork.CategoryBaseRepository.Update(category);
    //    await _unitOfWork.SaveChangesAsync();

    //    return new CategoryDto
    //    {
    //        Id = category.Id,
    //        Name = category.Name,
    //        Description = category.Description
    //    };
    //}
}
