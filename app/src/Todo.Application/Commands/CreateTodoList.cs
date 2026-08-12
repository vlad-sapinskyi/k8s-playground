using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Commands;

public record CreateTodoListCommand : IRequest<int>
{
    public string? Title { get; init; }
    public Colour? Colour { get; init; }
}

public class CreateTodoListCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTodoListCommand, int>
{
    public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList
        {
            Title = request.Title,
            Colour = request.Colour ?? Colour.Grey
        };

        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken) => 
        !await _context.TodoLists.AnyAsync(l => l.Title == title, cancellationToken);
}
