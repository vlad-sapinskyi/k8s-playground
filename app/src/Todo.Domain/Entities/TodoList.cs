using Todo.Domain.Common;
using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }
    public Colour Colour { get; set; }
    public IList<TodoItem> Items { get; private set; } = [];
}
