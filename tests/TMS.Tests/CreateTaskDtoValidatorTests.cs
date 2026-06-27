using FluentAssertions;
using FluentValidation.TestHelper;
using TMS.Application.DTOs;
using TMS.Application.Validators;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Tests;

public class CreateTaskDtoValidatorTests
{
    private readonly CreateTaskDtoValidator _validator = new();

    [Fact]
    public void Title_IsRequired_FailsWhenEmpty()
    {
        var dto = new CreateTaskDto { Title = "", Status = TaskStatus.ToDo, Priority = Priority.Low };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Title_MaxLength_FailsWhenExceeded()
    {
        var dto = new CreateTaskDto
        {
            Title    = new string('A', 201),
            Status   = TaskStatus.ToDo,
            Priority = Priority.Low
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Status_FailsWhenInvalidValue()
    {
        var dto = new CreateTaskDto
        {
            Title    = "Valid title",
            Status   = (TaskStatus)99,
            Priority = Priority.Low
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Priority_FailsWhenInvalidValue()
    {
        var dto = new CreateTaskDto
        {
            Title    = "Valid title",
            Status   = TaskStatus.ToDo,
            Priority = (Priority)99
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void ValidDto_PassesAllRules()
    {
        var dto = new CreateTaskDto
        {
            Title       = "Valid Task Title",
            Description = "Some description",
            Status      = TaskStatus.InProgress,
            Priority    = Priority.High,
            AssignedTo  = "alice"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
