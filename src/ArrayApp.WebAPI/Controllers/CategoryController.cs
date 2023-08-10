using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        try
        {
            var result = await _categoryService.CreateCategoryAsync(categoryCreateDto);
            return Ok(new ApiResponse<CategoryDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Category created successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("{categoryId}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetCategoryById(int categoryId)
    {
        try
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(new ApiResponse<CategoryDto>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Category retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    // Add similar endpoints for other methods in the CategoryService

    // Additional endpoints

    [HttpGet("totalcount")]
    [ProducesResponseType(typeof(ApiResponse<int>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTotalCategoryCount()
    {
        try
        {
            var result = await _categoryService.GetTotalCategoryCountAsync();
            return Ok(new ApiResponse<int>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Total category count retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("topcategories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTopCategories(int count)
    {
        try
        {
            var result = await _categoryService.GetTopCategoriesAsync(count);
            return Ok(new ApiResponse<IEnumerable<CategoryDto>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Top categories retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    // Add more endpoints as needed

}