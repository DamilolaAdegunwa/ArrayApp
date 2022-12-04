using ArrayApp.Application.Common.Mappings;
using ArrayApp.Domain.Entities;

namespace ArrayApp.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
