using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    IEnumerable<TodoItem> GetAllTodoItems();
    Task Add(TodoItemDto todoItemDto);
    Task MarkAsCompleted(int id);
    Task UpdateTodoItem(int id, TodoItemDto todoItemDto);
    IEnumerable<TodoItem> GetAllOpenTodoItems();
}