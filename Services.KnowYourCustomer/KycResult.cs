namespace Services.KnowYourCustomer
{
    public record KycResult(KycStatus KycStatus, Guid? ReportId = null);
}
