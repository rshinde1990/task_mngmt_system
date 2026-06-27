using FluentValidation;
using TMS.Application.DTOs;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Application.Validators;

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must not be empty when provided.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status!.Value)
                .Must(v => Enum.IsDefined(typeof(TaskStatus), v))
                .WithMessage("Status must be a valid TaskStatus value.");
        });

        When(x => x.Priority.HasValue, () =>
        {
            RuleFor(x => x.Priority!.Value)
                .Must(v => Enum.IsDefined(typeof(Priority), v))
                .WithMessage("Priority must be a valid Priority value.");
        });
    }
}
