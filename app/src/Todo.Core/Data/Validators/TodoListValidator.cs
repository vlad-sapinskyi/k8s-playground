using FluentValidation;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Enums;

namespace Todo.Core.Data.Validators;

public class TodoListValidator : AbstractValidator<TodoListDto>
{
    public TodoListValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Colour)
            .Must(c => string.IsNullOrEmpty(c) || Enum.TryParse<Colour>(c, out _))
            .WithMessage("Invalid 'Colour' value.");
    }
}
