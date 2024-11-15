using FluentValidation;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation
{
    public sealed class ProductOneValidator : AbstractValidator<Application>
    {
        public ProductOneValidator()
        {
            RuleFor(application => application.Applicant.DateOfBirth)
                .Must(BeWithinValidAgeRange)
                .WithErrorCode(ErrorConstants.ApplicantAgeInvalid)
                .WithMessage(ErrorConstants.ApplicantNameInvalidDescription);
        }

        private static bool BeWithinValidAgeRange(DateOnly dateOfBirth)
        {
            var age = CalculateAge(dateOfBirth);
            return age is >= 18 and <= 36;
        }

        private static int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;

            // Adjust if the birthday hasn't occurred yet this year
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
