using ArrayApp.Application.TodoLists.Queries.ExportTodos;

namespace ArrayApp.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
