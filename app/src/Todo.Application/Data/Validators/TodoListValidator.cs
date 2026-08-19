using FluentValidation;
using Todo.Application.Data.Dtos;
using Todo.Application.Data.Enums;

namespace Todo.Application.Data.Validators;

public class TodoListValidator : AbstractValidator<TodoListDto>
{
    public TodoListValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Colour)
            .NotEmpty()
            .Must(c => Enum.TryParse<Colour>(c, out _))
            .WithMessage("Invalid 'Colour' value.");
    }
}
