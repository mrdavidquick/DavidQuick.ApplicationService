using FluentAssertions;
using Moq;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.KnowYourCustomer;
using Xunit;
using IAdministratorTwo = Services.AdministratorTwo.Abstractions;

namespace Services.Applications.Tests;

[Collection("Non-Parallel Test Collection")]
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
        mockAdministratorTwo.Setup(x => x.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        var mockBus = new Mock<IBus>();

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);
        AdministratorServiceLocator.RegisterService<IBus>(mockBus.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

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
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

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
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

        var result = await processor.Process(application);

        mockAdministratorTwo.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Never);
        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.PaymentAmountInvalid);
        result.Error.Description.Should().Be(ErrorConstants.PaymentAmountInvalidDescription);
    }

    [Fact]
    public async Task Application_for_ProductTwo_returns_error_when_user_is_not_KYC_verified()
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
            .ReturnsAsync(new KycResult(KycStatus.NotVerified, Guid.NewGuid()));

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

        var result = await processor.Process(application);

        mockAdministratorTwo.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Never);
        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.KycNotVerified);
        result.Error.Description.Should().Be(ErrorConstants.KycNotVerifiedDescription);
    }

    [Fact]
    public async Task Application_for_ProductTwo_publishes_domain_events_to_the_bus_when_successful()
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
        mockAdministratorTwo.Setup(x => x.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        var mockBus = new Mock<IBus>();
        mockBus.Setup(x => x.PublishAsync(It.IsAny<InvestorCreated>()));
        mockBus.Setup(x => x.PublishAsync(It.IsAny<AccountCreated>()));
        mockBus.Setup(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()));

        AdministratorServiceLocator.RegisterService<IAdministratorTwo.IAdministrationService>(mockAdministratorTwo.Object);
        AdministratorServiceLocator.RegisterService<IBus>(mockBus.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, mockBus.Object);

        var result = await processor.Process(application);

        result.IsSuccess.Should().BeTrue();
        mockBus.Verify(b => b.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
        mockBus.Verify(b => b.PublishAsync(It.IsAny<AccountCreated>()), Times.Once);
        mockBus.Verify(b => b.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Once);
    }
}