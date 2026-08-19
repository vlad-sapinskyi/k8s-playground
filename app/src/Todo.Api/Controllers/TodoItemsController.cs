using Microsoft.AspNetCore.Mvc;
using Todo.Application.Data;
using Todo.Application.Data.Dtos;
using Todo.Application.Data.Entities;
using Todo.Application.Data.Enums;

[Route("api/todo/items")]
[ApiController]
public class TodoItemsController(IApplicationDbContext context) : ControllerBase
{
    // GET: api/todo/items/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([id], cancellationToken);

        return entity is null 
            ? NotFound() 
            : Ok(entity);
    }

    // POST: api/todo/items/
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> CreateTodoItem(TodoItemDto item, CancellationToken cancellationToken)
    {
        var entity = new TodoItemEntity
        {
            ListId = item.ListId,
            Title = item.Title,
            Done = false
        };

        context.TodoItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetTodoItem), new { id = entity.Id }, entity);
    }

    // PUT: api/todo/items/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(int id, TodoItemDto item, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([id], cancellationToken);

        if (entity is null) 
            return NotFound();

        entity.Title = item.Title;
        entity.Done = item.Done;
        entity.Priority = (PriorityLevel)item.Priority;
        entity.Note = item.Note;
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // DELETE: api/todo/items/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([id], cancellationToken);

        if (entity is null) 
            return NotFound();

        context.TodoItems.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
