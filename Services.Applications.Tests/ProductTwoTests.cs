using FluentAssertions;
using Moq;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Model;
using Services.KnowYourCustomer;
using Xunit;
using IAdministratorTwo = Services.AdministratorTwo.Abstractions;

namespace Services.Applications.Tests;

public class ProductTwoTests
{
    [Fact]
    public async Task Application_for_ProductTwo_creates_Investor_in_AdministratorTwo()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductTwo,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-20).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", 100m))
        };

        var mockAdministratorTwo = new Mock<IAdministratorTwo.IAdministrationService>();
        mockAdministratorTwo.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(KycStatus.Verified);

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object);

        var result = await processor.Process(application);

        mockAdministratorTwo.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Once);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Application_for_ProductTwo_returns_error_when_age_is_under_18()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductTwo,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-17).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", 100m))
        };

        var mockAdministratorTwo = new Mock<IAdministratorTwo.IAdministrationService>();
        mockAdministratorTwo.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(KycStatus.Verified);

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object);

        var result = await processor.Process(application);

        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.ApplicantAgeInvalid);
        result.Error.Description.Should().Be(ErrorConstants.ApplicantNameInvalidDescription);
    }

    [Fact]
    public async Task Application_for_ProductTwo_returns_error_when_payment_is_under_minimum()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductTwo,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-20).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", .98m))
        };

        var mockAdministratorTwo = new Mock<IAdministratorTwo.IAdministrationService>();
        mockAdministratorTwo.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(KycStatus.Verified);

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object);

        var result = await processor.Process(application);

        mockAdministratorTwo.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Never);
        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.PaymentAmountInvalid);
        result.Error.Description.Should().Be(ErrorConstants.PaymentAmountInvalidDescription);
    }

    [Fact]
    public async Task Application_for_ProductOne_returns_error_when_user_is_not_KYC_verified()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductOne,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-20).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", 100m))
        };

        var mockAdministratorTwo = new Mock<IAdministratorTwo.IAdministrationService>();
        mockAdministratorTwo.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(KycStatus.NotVerified);

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object);

        var result = await processor.Process(application);

        mockAdministratorTwo.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Never);
        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.KycNotVerified);
        result.Error.Description.Should().Be(ErrorConstants.KycNotVerifiedDescription);
    }
}