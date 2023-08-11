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
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<TagDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateTag(TagCreateDto tagCreateDto)
    {
        var createdTag = await _tagService.CreateTagAsync(tagCreateDto);
        return Ok(new ApiResponse<TagDto>
        {
            Code = SystemCodes.Successful,
            Data = createdTag,
            Description = "Tag created successfully",
        });
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TagDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Ok(new ApiResponse<IEnumerable<TagDto>>
        {
            Code = SystemCodes.Successful,
            Data = tags,
            Description = "Tags retrieved successfully",
        });
    }

    [HttpGet("idea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TagDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTagsByIdea(int ideaId)
    {
        var tags = await _tagService.GetTagsByIdeaAsync(ideaId);
        return Ok(new ApiResponse<IEnumerable<TagDto>>
        {
            Code = SystemCodes.Successful,
            Data = tags,
            Description = "Tags retrieved successfully",
        });
    }
}
