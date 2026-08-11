using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Models;
using Todo.Api.Data;
using Todo.Api.Dtos;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController(TodoContext context) : ControllerBase
{

    // GET: api/TodoItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItem() =>
        await context.TodoItems.Select(item => ToDto(item)).ToListAsync();

    // GET: api/TodoItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
    {
        var data = await context.TodoItems.FindAsync(id);

        return data == null ?
            (ActionResult<TodoItemDto>)NotFound() :
            (ActionResult<TodoItemDto>)ToDto(data);
    }

    // PUT: api/TodoItem/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long? id, TodoItemDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var data = await context.TodoItems.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }

        data.Name = dto.Name;
        data.IsComplete = dto.IsComplete;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/TodoItem
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto item)
    {
        var data = new TodoItem
        {
            IsComplete = item.IsComplete,
            Name = item.Name
        };

        context.TodoItems.Add(data);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = data.Id }, ToDto(data));
    }

    // DELETE: api/TodoItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long? id)
    {
        var data = await context.TodoItems.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }

        context.TodoItems.Remove(data);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long? id) =>
        context.TodoItems.Any(e => e.Id == id);

    private static TodoItemDto ToDto(TodoItem data) => new()
    {
       Id = data.Id,
       Name = data.Name,
       IsComplete = data.IsComplete
    };
}
