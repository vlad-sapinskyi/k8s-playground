using FluentValidation.TestHelper;
using Todo.Core.Data.Dtos;
using Todo.Core.Data.Enums;
using Todo.Core.Data.Validators;
using Xunit;

namespace Todo.Core.UnitTests.Data.Validators;

public class TodoListValidatorTests
{
    private readonly TodoListValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Title_NullEmptyOrWhitespace_HasValidationError(string? title)
    {
        // Arrange
        var dto = new TodoListDto { Title = title! };

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
        var dto = new TodoListDto { Title = new string('a', length) };

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
    [InlineData(null)]
    [InlineData("")]
    public void Colour_NullOrEmpty_HasNoValidationError(string? colour)
    {
        // Arrange
        var dto = new TodoListDto { Title = "Valid title", Colour = colour! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Colour);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(nameof(Colour.Red), false)]
    [InlineData(nameof(Colour.Orange), false)]
    [InlineData(nameof(Colour.Green), false)]
    [InlineData(nameof(Colour.Blue), false)]
    [InlineData(nameof(Colour.Purple), false)]
    [InlineData(nameof(Colour.Grey), false)]
    [InlineData("WrongColour", true)]
    [InlineData("red", true)]
    public void Colour_EnumValue_ValidatesCorrectly(string? colour, bool expectError)
    {
        // Arrange
        var dto = new TodoListDto { Title = "Valid title", Colour = colour! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (expectError)
        {
            result.ShouldHaveValidationErrorFor(x => x.Colour)
                .WithErrorMessage("Invalid 'Colour' value.");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Colour);
        }
    }
}
