using Todo.Domain.Common;
using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class TodoListEntity : BaseEntity
{
    public string? Title { get; set; }
    public Colour Colour { get; set; }
    public IList<TodoItemEntity> Items { get; private set; } = [];
}
