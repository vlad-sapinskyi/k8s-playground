using Todo.Core.Data.Enums;

namespace Todo.Core.Data.Entities;

public class TodoListEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public Colour Colour { get; set; } = Colour.Grey;

    public ICollection<TodoItemEntity> Items { get; private set; } = [];
}
