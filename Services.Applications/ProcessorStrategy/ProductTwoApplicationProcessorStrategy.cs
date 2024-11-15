using FluentValidation;
using FluentValidation.Results;
using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public sealed class ProductTwoApplicationProcessorStrategy
    (AdministratorTwo.Abstractions.IAdministrationService administrationService, IValidator<Application> validator) 
    : IApplicationProcessorStrategy
{
    public async Task<Result<InvestorAccount>> Process(Application application)
    {
        var validatorResult = await validator.ValidateAsync(application);

        if (!validatorResult.IsValid) return await Task.FromResult(Result.Failure<InvestorAccount>(GetFirstValidationError(validatorResult.Errors)));

        var user = new User();

        _ = await administrationService.CreateInvestorAsync(user);

        return await Task.FromResult(Result.Success(
            new InvestorAccount(AdministratorCode.AdministratorOne)));
    }

    private Error GetFirstValidationError(List<ValidationFailure> validationFailures)
    {
        var firstFailure = validationFailures.First();
        return new Error(Constants.SystemName, firstFailure.ErrorCode, firstFailure.ErrorMessage);
    }
}