using School.Application.DTOs;
using FluentValidation;

namespace School.Application.Validators;

public class SubjectRequestValidator : AbstractValidator<SubjectRequestDTO>
{
    public SubjectRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.WeeklyHours).InclusiveBetween((byte)1, (byte)40).When(x => x.WeeklyHours.HasValue);
    }
}
