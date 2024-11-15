using FluentValidation;
using FluentValidation.Results;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public sealed class ProductOneApplicationProcessorStrategy
    (AdministratorOne.Abstractions.IAdministrationService administrationService, IValidator<Application> validator) 
    : IApplicationProcessorStrategy
{
    public Task<Result<InvestorAccount>> Process(Application application)
    {
        var validatorResult = validator.Validate(application);

        if (!validatorResult.IsValid) return Task.FromResult(Result.Failure<InvestorAccount>(GetFirstValidationError(validatorResult.Errors)));

        _ = administrationService.CreateInvestor(new CreateInvestorRequest());

        return Task.FromResult(Result.Success(
            new InvestorAccount(AdministratorCode.AdministratorOne)));
    }

    private Error GetFirstValidationError(List<ValidationFailure> validationFailures)
    {
        var firstFailure = validationFailures.First();
        return new Error(Constants.SystemName, firstFailure.ErrorCode, firstFailure.ErrorMessage);
    }
}