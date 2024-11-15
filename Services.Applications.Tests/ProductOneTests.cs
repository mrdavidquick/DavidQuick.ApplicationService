using FluentAssertions;
using Moq;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Model;
using Xunit;
using IAdministratorOne = Services.AdministratorOne.Abstractions;

namespace Services.Applications.Tests;

public class ProductOneTests
{
    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne()
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

        var mockAdministratorOne = new Mock<IAdministratorOne.IAdministrationService>();
        mockAdministratorOne.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(new CreateInvestorResponse());

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory);

        var result = await processor.Process(application);

        mockAdministratorOne.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Once);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Application_for_ProductOne_returns_error_when_age_is_over_36()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductOne,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-37).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", 100m))
        };

        var mockAdministratorOne = new Mock<IAdministratorOne.IAdministrationService>();
        mockAdministratorOne.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(new CreateInvestorResponse());

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory);

        var result = await processor.Process(application);

        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.ApplicantAgeInvalid);
        result.Error.Description.Should().Be(ErrorConstants.ApplicantNameInvalidDescription);
    }

    [Fact]
    public async Task Application_for_ProductOne_returns_error_when_age_is_under_18()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductOne,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-17).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", 100m))
        };

        var mockAdministratorOne = new Mock<IAdministratorOne.IAdministrationService>();
        mockAdministratorOne.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(new CreateInvestorResponse());

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory);

        var result = await processor.Process(application);

        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.ApplicantAgeInvalid);
        result.Error.Description.Should().Be(ErrorConstants.ApplicantNameInvalidDescription);
    }

    [Fact]
    public async Task Application_for_ProductOne_returns_error_when_payment_is_under_minimum()
    {
        var application = new Application
        {
            Id = Guid.NewGuid(),
            ProductCode = ProductCode.ProductOne,
            Applicant = new User
            {
                DateOfBirth = new DateOnly(DateTime.Today.AddYears(-20).Year, 1, 1)
            },
            Payment = new Payment(new BankAccount(), new Money("", .98m))
        };

        var mockAdministratorOne = new Mock<IAdministratorOne.IAdministrationService>();
        mockAdministratorOne.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(new CreateInvestorResponse());

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory);

        var result = await processor.Process(application);

        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.PaymentAmountInvalid);
        result.Error.Description.Should().Be(ErrorConstants.PaymentAmountInvalidDescription);
    }
}