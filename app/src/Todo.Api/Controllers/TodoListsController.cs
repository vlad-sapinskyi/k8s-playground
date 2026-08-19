using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Data;
using Todo.Application.Data.Dtos;
using Todo.Application.Data.Entities;
using Todo.Application.Data.Enums;

[Route("api/todo/lists")]
[ApiController]
public class TodoListsController(IApplicationDbContext context, IMapper mapper) : ControllerBase
{
    // GET: api/todo/lists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListDto>>> GetTodoLists(CancellationToken cancellationToken)
    {
        var dtos = await context.TodoLists
            .AsNoTracking()
            .ProjectTo<TodoListDto>(mapper.ConfigurationProvider)
            .OrderBy(list => list.Title)
            .ToListAsync(cancellationToken);

        return Ok(dtos);
    }

    // GET: api/todo/lists/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoList(int id, CancellationToken cancellationToken)
    {
        var dto = await context.TodoLists
            .AsNoTracking()
            .Where(list => list.Id == id)
            .ProjectTo<TodoListDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return dto is null 
            ? NotFound() 
            : Ok(dto);
    }

    // POST: api/todo/lists/
    [HttpPost]
    public async Task<ActionResult<TodoListDto>> CreateTodoList(TodoListDto list, CancellationToken cancellationToken)
    {
        var entity = new TodoListEntity
        {
            Title = list.Title,
            Colour = Enum.TryParse(list.Colour, out Colour colour) ? colour : default
        };

        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetTodoList), new { id = entity.Id }, entity);
    }

    // PUT: api/todo/lists/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoList(int id, TodoListDto list, CancellationToken cancellationToken)
    {
        var entity = await context.TodoLists.FindAsync([id], cancellationToken);

        if (entity is null) 
            return NotFound();

        entity.Title = list.Title;
        entity.Colour = Enum.TryParse(list.Colour, out Colour colour) ? colour : default;

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // DELETE: api/todo/lists/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id, CancellationToken cancellationToken)
    {
        var entity = await context.TodoLists.FindAsync([id], cancellationToken);

        if (entity is null) 
            return NotFound();

        context.TodoLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
