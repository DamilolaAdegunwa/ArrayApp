using System.Collections.Generic;

namespace ArrayApp.Application.Common.Models;

public class UserCreatedListModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UserCreatedListResponse
{
    public List<UserCreatedListModel> Lists { get; set; } = new();
}
