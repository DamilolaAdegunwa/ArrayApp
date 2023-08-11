using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateProduct(ProductCreateDto productCreateDto)
    {
        var createdProduct = await _productService.CreateProductAsync(productCreateDto);
        return Ok(new ApiResponse<ProductDto>
        {
            Code = SystemCodes.Successful,
            Data = createdProduct,
            Description = "Product created successfully",
        });
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetProductById(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        return Ok(new ApiResponse<ProductDto>
        {
            Code = SystemCodes.Successful,
            Data = product,
            Description = "Product retrieved successfully",
        });
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(new ApiResponse<IEnumerable<ProductDto>>
        {
            Code = SystemCodes.Successful,
            Data = products,
            Description = "Products retrieved successfully",
        });
    }

    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(new ApiResponse<IEnumerable<ProductDto>>
        {
            Code = SystemCodes.Successful,
            Data = products,
            Description = "Products retrieved successfully",
        });
    }

}
