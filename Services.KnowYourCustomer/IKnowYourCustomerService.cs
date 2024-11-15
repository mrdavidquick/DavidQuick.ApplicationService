using Services.Common.Abstractions.Model;

namespace Services.KnowYourCustomer;

public interface IKnowYourCustomerService
{
    Task<KycStatus> PerformKycCheck(User user);
}