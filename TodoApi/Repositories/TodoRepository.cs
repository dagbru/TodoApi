using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository(IDbContextFactory<TodoDbContext> contextFactory) : ITodoRepository
{
    private readonly TodoDbContext _dbContext = contextFactory.CreateDbContext();

    public IEnumerable<TodoItem> GetAllTodoItems()
    {
        return _dbContext.TodoItems;
    }

    public async Task Add(TodoItemDto todoItemDto)
    {
        var todoItem = new TodoItem
        {
            Text = todoItemDto.Text,
            Title = todoItemDto.Title,
            CreatedAt = DateTime.UtcNow,
            Completed = false
        };
        
        _dbContext.TodoItems.Add(todoItem);
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task<TodoItem> GetTodoItemById(int id)
    {
        return await _dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task MarkAsCompleted(int id)
    {
        var existing = await GetTodoItemById(id);
        existing.Completed = true;
        existing.CompletedAt = DateTime.UtcNow;
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTodoItem(int id, TodoItemDto todoItemDto)
    {
        var existing = await GetTodoItemById(id);
        existing.UpdatedAt = DateTime.UtcNow;
        existing.Text = todoItemDto.Text;
        existing.Title = todoItemDto.Title;

        await _dbContext.SaveChangesAsync();
    }

    public IEnumerable<TodoItem> GetAllOpenTodoItems()
    {
        return _dbContext.TodoItems.Where(x => !x.Completed);
    }
}