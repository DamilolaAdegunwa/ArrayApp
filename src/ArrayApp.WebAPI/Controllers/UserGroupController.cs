using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserGroupController : ControllerBase
{
    private readonly IUserGroupService _userGroupService;

    public UserGroupController(IUserGroupService userGroupService)
    {
        _userGroupService = userGroupService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateUserGroup(UserGroupCreateDto userGroupCreateDto)
    {
        var createdUserGroup = await _userGroupService.CreateUserGroupAsync(userGroupCreateDto);
        return Ok(new ApiResponse<UserGroupDto>
        {
            Code = SystemCodes.Successful,
            Data = createdUserGroup,
            Description = "User group created successfully",
        });
    }

    [HttpGet("get/{userGroupId}")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserGroupById(int userGroupId)
    {
        var userGroup = await _userGroupService.GetUserGroupByIdAsync(userGroupId);
        return Ok(new ApiResponse<UserGroupDto>
        {
            Code = SystemCodes.Successful,
            Data = userGroup,
            Description = "User group retrieved successfully",
        });
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserGroupDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllUserGroups()
    {
        var userGroups = await _userGroupService.GetAllUserGroupsAsync();
        return Ok(new ApiResponse<IEnumerable<UserGroupDto>>
        {
            Code = SystemCodes.Successful,
            Data = userGroups,
            Description = "All user groups retrieved successfully",
        });
    }

    [HttpPut("update/{userGroupId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserGroup(int userGroupId, UserGroupUpdateDto userGroupUpdateDto)
    {
        await _userGroupService.UpdateUserGroupAsync(userGroupId, userGroupUpdateDto);
        return Ok(new ApiResponse<string>
        {
            Code = SystemCodes.Successful,
            Data = "User group updated successfully",
            Description = "User group updated successfully",
        });
    }

    [HttpDelete("delete/{userGroupId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteUserGroup(int userGroupId)
    {
        await _userGroupService.DeleteUserGroupAsync(userGroupId);
        return Ok(new ApiResponse<string>
        {
            Code = SystemCodes.Successful,
            Data = "User group deleted successfully",
            Description = "User group deleted successfully",
        });
    }

    // Add more endpoints for other methods in the UserGroupService
}
