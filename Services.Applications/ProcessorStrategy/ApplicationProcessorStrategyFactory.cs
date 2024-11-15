using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public class ApplicationProcessorStrategyFactory : IApplicationProcessorStrategyFactory
{
    public IApplicationProcessorStrategy Create(ProductCode productCode)
    {
        return new ProductOneApplicationProcessorStrategy(AdministratorServiceLocator.GetService<AdministratorOne.Abstractions.IAdministrationService>());
    }
}