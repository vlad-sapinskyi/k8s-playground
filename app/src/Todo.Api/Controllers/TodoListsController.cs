using Microsoft.AspNetCore.Mvc;
using Todo.Core.Data.Dtos;
using Todo.Core.Services;

namespace Todo.Api.Controllers;

[Route("api/todo/lists")]
[ApiController]
public class TodoListsController(ITodoListService service) : ControllerBase
{
    // GET: api/todo/lists
    [HttpGet]
    public async Task<ActionResult<TodoListDto[]>> GetTodoLists(CancellationToken cancellationToken)
    {
        var dtos = await service.GetAllAsync(cancellationToken);

        return Ok(dtos);
    }

    // GET: api/todo/lists/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoList(int id, CancellationToken cancellationToken)
    {
        var dto = await service.GetByIdAsync(id, cancellationToken);

        return dto is null ? NotFound() : Ok(dto);
    }

    // POST: api/todo/lists/
    [HttpPost]
    public async Task<IActionResult> CreateTodoList(TodoListDto list, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(list, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetTodoList), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Errors);
    }

    // PUT: api/todo/lists/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoList(int id, TodoListDto list, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, list, cancellationToken);

        return result.Succeeded ? NoContent() : NotFound(result.Errors);
    }

    // DELETE: api/todo/lists/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);

        return result.Succeeded ? NoContent() : NotFound(result.Errors);
    }
}
