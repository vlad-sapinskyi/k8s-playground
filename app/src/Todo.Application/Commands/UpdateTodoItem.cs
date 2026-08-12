using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Todo.Application.Common;

namespace Todo.Application.Commands;

public record UpdateTodoItemCommand : IRequest
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateTodoItemCommand>
{
    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems.FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Done = request.Done;

        await context.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator() =>
        RuleFor(v => v.Title)
        .MaximumLength(200)
        .NotEmpty();
}
