using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public class ApplicationProcessorStrategyFactory : IApplicationProcessorStrategyFactory
{
    public IApplicationProcessorStrategy Create(ProductCode productCode)
    {
        return productCode switch
        {
            ProductCode.ProductOne => new ProductOneApplicationProcessorStrategy(
                AdministratorServiceLocator.GetService<AdministratorOne.Abstractions.IAdministrationService>(),
                new ProductOneValidator(),
                AdministratorServiceLocator.GetService<IBus>()),
            ProductCode.ProductTwo => new ProductTwoApplicationProcessorStrategy(
                AdministratorServiceLocator.GetService<AdministratorTwo.Abstractions.IAdministrationService>(),
                new ProductTwoValidator()),
            _ => throw new ArgumentOutOfRangeException(nameof(productCode), productCode, null)
        };
    }
}