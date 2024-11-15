using Services.Common.Abstractions.Model;

namespace Services.KnowYourCustomer;

public interface IKnowYourCustomerService
{
    Task<KycResult> PerformKycCheck(User user);
}