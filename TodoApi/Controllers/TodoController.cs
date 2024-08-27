using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;


namespace TodoApi.Controllers;

[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _todoRepository;

    public TodoController(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }
    
    [HttpPost("add")]
    public async Task<IActionResult> AddNewTodo([FromBody] TodoItemDto todoItemDto)
    {
        try
        {
            await _todoRepository.Add(todoItemDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] TodoItemDto todoItemDto)
    {
        await _todoRepository.UpdateTodoItem(id, todoItemDto);
        return Ok();
    }

    [HttpPut("complete/{id:int}")]
    public async Task<IActionResult> MarkAsCompleted(int id)
    {
        await _todoRepository.MarkAsCompleted(id);

        return Ok();
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<TodoItem>> GetAllItems()
    {
        return Ok(_todoRepository.GetAllTodoItems());
    }

    [HttpGet("open")]
    public ActionResult<IEnumerable<TodoItem>> GetAllOpenItems()
    {
        return Ok(_todoRepository.GetAllOpenTodoItems());
    }
}