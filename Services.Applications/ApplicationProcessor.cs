using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor(IApplicationProcessorStrategyFactory applicationProcessorStrategyFactory) : IApplicationProcessor
{
    public async Task<Result<InvestorAccount>> Process(Application application)
    {
        var processStrategy = applicationProcessorStrategyFactory.Create(application.ProductCode);
        var applicationProcessResult = await processStrategy.Process(application);

        return applicationProcessResult;
    }
}