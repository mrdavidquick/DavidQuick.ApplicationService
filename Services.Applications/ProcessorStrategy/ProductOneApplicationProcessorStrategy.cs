using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public class ProductOneApplicationProcessorStrategy(AdministratorOne.Abstractions.IAdministrationService administrationService) : IApplicationProcessorStrategy
{
    public Task<Result<InvestorAccount>> Process(Application application)
    {
        var response = administrationService.CreateInvestor(new CreateInvestorRequest());

        return Task.FromResult(Result.Success(
            new InvestorAccount(AdministratorCode.AdministratorOne)));
    }
}