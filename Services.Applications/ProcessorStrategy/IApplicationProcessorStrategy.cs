using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public interface IApplicationProcessorStrategy
{
    Task<Result<InvestorAccount>> Process(Application application);
}