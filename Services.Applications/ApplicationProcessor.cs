using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.KnowYourCustomer;

namespace Services.Applications;

public class ApplicationProcessor(
    IApplicationProcessorStrategyFactory applicationProcessorStrategyFactory,
    IKnowYourCustomerService kycService,
    IBus bus) : IApplicationProcessor
{
    public async Task<Result<InvestorAccount>> Process(Application application)
    {
        var kycResult = await kycService.PerformKycCheck(application.Applicant);

        if (kycResult.KycStatus is KycStatus.NotVerified)
        {
            await bus.PublishAsync(new KycFailed(application.Applicant.Id, kycResult.ReportId!.Value)); // really need to enforce ReportId not being null
            return Result.Failure<InvestorAccount>(
                new Error(Constants.SystemName, ErrorConstants.KycNotVerified, ErrorConstants.KycNotVerifiedDescription));
        }

        var processStrategy = applicationProcessorStrategyFactory.Create(application.ProductCode);
        var applicationProcessResult = await processStrategy.Process(application);

        await bus.PublishAsync(new ApplicationCompleted(application.Id));

        return applicationProcessResult;
    }
}