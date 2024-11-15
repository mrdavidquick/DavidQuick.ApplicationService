using Services.Common.Abstractions.Model;

namespace Services.Common.Abstractions.Abstractions;

public interface IApplicationProcessor  
{
    Task<Result<InvestorAccount>> Process(Application application);
}