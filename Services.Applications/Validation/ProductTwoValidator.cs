using FluentValidation;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation
{
    public sealed class ProductTwoValidator : AbstractValidator<Application>
    {
        public ProductTwoValidator()
        {
            RuleFor(application => application.Applicant.DateOfBirth)
                .Must(BeWithinValidAgeRange)
                .WithErrorCode(ErrorConstants.ApplicantAgeInvalid)
                .WithMessage(ErrorConstants.ApplicantNameInvalidDescription);

            RuleFor(application => application.Payment)
                .Must(BeWithinValidPaymentRange)
                .WithErrorCode(ErrorConstants.PaymentAmountInvalid)
                .WithMessage(ErrorConstants.PaymentAmountInvalidDescription);
        }

        private static bool BeWithinValidAgeRange(DateOnly dateOfBirth)
        {
            var age = CalculateAge(dateOfBirth);
            return age is >= 18;
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

        private static bool BeWithinValidPaymentRange(Payment payment)
        {
            return payment.Amount.Amount is >= 0.99m;
        }
    }
}