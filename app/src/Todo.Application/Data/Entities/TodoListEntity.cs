using Todo.Application.Data.Enums;

namespace Todo.Application.Data.Entities;

public class TodoListEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public Colour Colour { get; set; } = Colour.Grey;

    public ICollection<TodoItemEntity> Items { get; private set; } = [];
}
