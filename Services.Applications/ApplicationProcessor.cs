using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor : IApplicationProcessor
{
    public async Task<Result<InvestorAccount>> Process(Application application)
    {
        throw new NotImplementedException();
    }
}