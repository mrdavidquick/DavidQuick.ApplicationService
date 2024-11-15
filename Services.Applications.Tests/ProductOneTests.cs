using FluentAssertions;
using Moq;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.ProcessorStrategy;
using Xunit;
using IAdministratorOne = Services.AdministratorOne.Abstractions;

namespace Services.Applications.Tests;

public class ProductOneTests
{
    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne()
    {
        var application = ApplicationTestHelper.GetEmptyProductOneApplication();

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
}