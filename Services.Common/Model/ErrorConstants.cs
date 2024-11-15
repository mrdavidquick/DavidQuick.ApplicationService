namespace Services.Common.Abstractions.Model
{
    public sealed class ErrorConstants
    {
        public const string ApplicantAgeInvalid = "Applicant.AgeInvalid";
        public const string ApplicantNameInvalidDescription = "Applicant's age must be between 18 and 36 years.";

        public const string PaymentAmountInvalid = "Payment.AmountInvalid";
        public const string PaymentAmountInvalidDescription = "The minimum payment is £0.99.";
    }
}
