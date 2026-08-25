using FluentValidation;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Enums;

namespace Todo.Core.Data.Validators;

public class TodoItemValidator : AbstractValidator<TodoItemDto>
{
    public TodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Note)
            .MaximumLength(1000);

        RuleFor(x => x.Priority)
            .Must(p => Enum.IsDefined((PriorityLevel)p))
            .WithMessage("Invalid 'Priority' value.");
    }
}
