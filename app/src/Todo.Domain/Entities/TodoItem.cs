namespace Todo.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public int ListId { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool IsDone { get; set; }
    public TodoList List { get; set; } = null!;
}
