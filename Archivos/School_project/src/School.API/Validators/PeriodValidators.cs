using School.Application.DTOs;
using FluentValidation;

namespace School.Application.Validators;

public class PeriodRequestValidator : AbstractValidator<PeriodRequestDTO>
{
    public PeriodRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(20);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
        RuleFor(x => x.Status).NotEmpty().Must(s => s is "Open" or "Closed");
    }
}
