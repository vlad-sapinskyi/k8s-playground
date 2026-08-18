using FluentValidation;
using MediatR;
using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Commands;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }
    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTodoItemCommand, int>
{
    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItemEntity
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        context.TodoItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator() =>
        RuleFor(v => v.Title)
        .MaximumLength(200)
        .NotEmpty();
}
