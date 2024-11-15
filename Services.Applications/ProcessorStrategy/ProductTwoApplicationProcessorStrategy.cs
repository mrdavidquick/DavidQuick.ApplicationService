using FluentValidation;
using FluentValidation.Results;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications.ProcessorStrategy;

public sealed class ProductTwoApplicationProcessorStrategy
    (AdministratorTwo.Abstractions.IAdministrationService administrationService, 
        IValidator<Application> validator,
        IBus serviceBus) 
    : IApplicationProcessorStrategy
{
    public async Task<Result<InvestorAccount>> Process(Application application)
    {
        var validatorResult = await validator.ValidateAsync(application);

        if (!validatorResult.IsValid) return await Task.FromResult(Result.Failure<InvestorAccount>(GetFirstValidationError(validatorResult.Errors)));

        var user = new User();

        var investorResult = await administrationService.CreateInvestorAsync(user);

        if(!investorResult.IsSuccess) return Result.Failure<InvestorAccount>(new Error("","", ""));

        await serviceBus.PublishAsync(new InvestorCreated(application.Applicant.Id, investorResult.Value.ToString()));

        var accountResult = await administrationService.CreateAccountAsync(investorResult.Value, application.ProductCode);
        if (!accountResult.IsSuccess) return Result.Failure<InvestorAccount>(new Error("", "", ""));

        await serviceBus.PublishAsync(new AccountCreated(investorResult.Value.ToString(), application.ProductCode,
            accountResult.Value.ToString()));

        _ = await administrationService.ProcessPaymentAsync(accountResult.Value, application.Payment);

        return await Task.FromResult(Result.Success(
            new InvestorAccount(AdministratorCode.AdministratorOne)));
    }

    private Error GetFirstValidationError(List<ValidationFailure> validationFailures)
    {
        var firstFailure = validationFailures.First();
        return new Error(Constants.SystemName, firstFailure.ErrorCode, firstFailure.ErrorMessage);
    }
}