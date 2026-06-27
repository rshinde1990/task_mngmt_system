using FluentValidation;
using TMS.Application.DTOs;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Application.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .Must(v => Enum.IsDefined(typeof(TaskStatus), v))
            .WithMessage("Status must be a valid TaskStatus value.");

        RuleFor(x => x.Priority)
            .Must(v => Enum.IsDefined(typeof(Priority), v))
            .WithMessage("Priority must be a valid Priority value.");
    }
}
