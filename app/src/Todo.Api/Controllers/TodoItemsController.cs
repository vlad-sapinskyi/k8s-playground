using Microsoft.AspNetCore.Mvc;
using Todo.Application.Data;
using Todo.Application.Data.Dtos;
using Todo.Application.Data.Entities;
using Todo.Application.Data.Enums;
using Todo.Application.Services;

[Route("api/todo/items")]
[ApiController]
public class TodoItemsController(ITodoItemService service) : ControllerBase
{
    // GET: api/todo/items/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(int id, CancellationToken cancellationToken)
    {
        var dto = await service.GetByIdAsync(id, cancellationToken);

        return dto is null ? NotFound() : Ok(dto);
    }

    // POST: api/todo/items/
    [HttpPost]
    public async Task<IActionResult> CreateTodoItem(TodoItemDto item, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(item, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetTodoItem), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Errors);
    }

    // PUT: api/todo/items/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(int id, TodoItemDto item, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, item, cancellationToken);

        return result.Succeeded ? NoContent() : NotFound(result.Errors);
    }

    // DELETE: api/todo/items/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);

        return result.Succeeded ? NoContent() : NotFound(result.Errors);
    }
}
