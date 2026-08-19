using Todo.Application.Data.Enums;

namespace Todo.Application.Data.Entities;

public class TodoListEntity : BaseEntity
{
    public string? Title { get; set; }
    public Colour Colour { get; set; } = Colour.Grey;
    public IList<TodoItemEntity> Items { get; private set; } = [];
}
