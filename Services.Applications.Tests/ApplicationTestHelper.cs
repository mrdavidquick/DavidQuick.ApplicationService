using Services.Common.Abstractions.Model;

namespace Services.Applications.Tests
{
    internal static class ApplicationTestHelper
    {
        private const decimal InitialMinimumPayment = 0.99m;
        private const decimal InitialMaximumPayment = 4000.00m;

        internal static decimal GenerateRandomPayment()
        {
            var random = new Random();
            const double range = (double)(InitialMaximumPayment - InitialMinimumPayment);
            var sample = random.NextDouble() * range + (double)InitialMinimumPayment;
            return (decimal)sample;
        }

        internal static Application GetEmptyProductOneApplication()
        {
            return new Application
            {
                Id = Guid.NewGuid(),
                ProductCode = ProductCode.ProductOne,
            };
        }
    }
}

