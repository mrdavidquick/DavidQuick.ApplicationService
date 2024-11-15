using Services.Common.Abstractions.Model;
using Services.Common.Abstractions.Model.ResourceType;

namespace Services.Applications.ProcessorStrategy
{
    public interface IApplicationProcessorStrategyFactory
    {
        IApplicationProcessorStrategy Create(ProductCode productCode);
    }
}
