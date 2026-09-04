using FluentValidation.TestHelper;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Enums;
using Todo.Core.Data.Validators;
using Xunit;

namespace Todo.Core.UnitTests.Data.Validators;

public class TodoItemValidatorTests
{
    private readonly TodoItemValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Title_NullEmptyOrWhitespace_HasValidationError(string? title)
    {
        // Arrange
        var dto = new TodoItemDto { Title = title! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(199, false)]
    [InlineData(200, false)]
    [InlineData(201, true)]
    public void Title_Lenght_ValidatesCorrectly(int length, bool expectError)
    {
        // Arrange
        var dto = new TodoItemDto { Title = new string('a', length) };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (expectError)
        {
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Note_NullEmptyOrWhitespace_HasNoValidationError(string? note)
    {
        // Arrange
        var dto = new TodoItemDto { Title = "Valid title", Note = note };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Note);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, false)]
    [InlineData(1001, true)]
    public void Note_Lenght_ValidatesCorrectly(int length, bool expectError)
    {
        // Arrange
        var dto = new TodoItemDto { Note = new string('a', length) };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (expectError)
        {
            result.ShouldHaveValidationErrorFor(x => x.Note);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Note);
        }
    }

    [Theory]
    [InlineData(PriorityLevel.None, false)]
    [InlineData(PriorityLevel.Low, false)]
    [InlineData(PriorityLevel.Medium, false)]
    [InlineData(PriorityLevel.High, false)]
    [InlineData(999, true)]
    public void Priority_EnumValue_ValidatesCorrectly(PriorityLevel priority, bool expectError)
    {
        // Arrange
        var dto = new TodoItemDto { Title = "Valid title", Priority = (int)priority };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (expectError)
        {
            result.ShouldHaveValidationErrorFor(x => x.Priority)
                .WithErrorMessage("Invalid 'Priority' value.");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }
    }
}
