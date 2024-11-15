using FluentAssertions;
using Moq;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.ProcessorStrategy;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.KnowYourCustomer;
using Xunit;
using IAdministratorOne = Services.AdministratorOne.Abstractions;

namespace Services.Applications.Tests;

[Collection("Non-Parallel Test Collection")]
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

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);
        AdministratorServiceLocator.RegisterService<IBus>(new Mock<IBus>().Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

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

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);
        AdministratorServiceLocator.RegisterService<IBus>(new Mock<IBus>().Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

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

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);
        AdministratorServiceLocator.RegisterService<IBus>(new Mock<IBus>().Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

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

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, new Mock<IBus>().Object);

        var result = await processor.Process(application);

        mockAdministratorOne.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
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

        var mockAdministratorOne = new Mock<IAdministratorOne.IAdministrationService>();
        mockAdministratorOne.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(new CreateInvestorResponse());

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.NotVerified, Guid.NewGuid()));

        var mockBus = new Mock<IBus>();

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);

        var applicationProcessorStrategyFactory = new ApplicationProcessorStrategyFactory();

        var processor = new ApplicationProcessor(applicationProcessorStrategyFactory, mockKycService.Object, mockBus.Object);

        var result = await processor.Process(application);

        mockAdministratorOne.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
        mockBus.Verify(b => b.PublishAsync(It.IsAny<KycFailed>()), Times.Once);
        result.IsSuccess.Should().BeFalse();
        result.Error.System.Should().Be(Constants.SystemName);
        result.Error.Code.Should().Be(ErrorConstants.KycNotVerified);
        result.Error.Description.Should().Be(ErrorConstants.KycNotVerifiedDescription);
    }

    [Fact]
    public async Task Application_for_ProductOne_publishes_domain_events_to_the_bus_when_successful()
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

        var mockKycService = new Mock<IKnowYourCustomerService>();
        mockKycService.Setup(x => x.PerformKycCheck(It.IsAny<User>()))
            .ReturnsAsync(new KycResult(KycStatus.Verified));

        var mockBus = new Mock<IBus>();
        mockBus.Setup(x => x.PublishAsync(It.IsAny<InvestorCreated>()));
        mockBus.Setup(x => x.PublishAsync(It.IsAny<AccountCreated>()));
        mockBus.Setup(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()));

        AdministratorServiceLocator.RegisterService<IAdministratorOne.IAdministrationService>(mockAdministratorOne.Object);
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