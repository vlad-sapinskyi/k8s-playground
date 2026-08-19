using Todo.Application.Data.Enums;

namespace Todo.Application.Data.Entities;

public class TodoItemEntity : BaseEntity
{
    public int ListId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public bool Done { get; set; }

    public TodoListEntity List { get; set; } = null!;
}
