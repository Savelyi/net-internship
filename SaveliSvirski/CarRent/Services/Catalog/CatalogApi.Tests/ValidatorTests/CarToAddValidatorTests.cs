using CQRS.Commands;
using FluentValidation.TestHelper;
using Validator;
using Xunit;

namespace CatalogApi.Tests.ValidatorTests
{
    public class CarToAddValidatorTests
    {
        private readonly CarToAddValidator validator;

        public CarToAddValidatorTests()
        {
            validator = new CarToAddValidator();
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData("newModel", "newMake", 100, null)]
        [InlineData("newModel", "newMake", null, "1337HR-7")]
        [InlineData("", "", 100, "1337HR-7")]
        public void ValidateCarToAddCommand_ShouldFail_WhenAnyOfTheFieldsIsNullOrEmpty(string model, string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("NotEmptyValidator");
        }

        [Theory]
        [InlineData("newModel", "newMake", 100, "1337HR-79")]
        [InlineData("newModel", "newMake", 100, "1337HR-")]
        [InlineData("newModel", "newMake", 100, "1337H")]
        [InlineData("newModel", "newMake", 100, "13R-7")]
        public void ValidateCarToAddCommand_ShouldFail_WhenVehicleNumberLengthLessOrMoreThan8Characterrs(string model,
            string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("ExactLengthValidator");
        }

        [Theory]
        [InlineData("newModel", "newMake", 100, "1337HRR7")]
        [InlineData("newModel", "newMake", 100, "13375-HR")]
        [InlineData("newModel", "newMake", 100, "1337H-HT")]
        public void ValidateCarToAddCommand_ShouldFail_WhenVehicleNumberDoesNotMatchThePattern(string model,
            string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("RegularExpressionValidator");
        }

        [Theory]
        [InlineData("newModel", "newMake", 100, "1337HR-7")]
        [InlineData("newModel", "newMake", 100, "4567BR-5")]
        [InlineData("newModel", "newMake", 100, "8193TA-3")]
        public void ValidateCarToAddCommand_ShouldNotFail_WhenVehicleNumberMatchesThePattern(string model, string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldNotHaveAnyValidationErrors();
        }

        private CarToAddCommand GetTestCarCommand(string model, string make,
            decimal rentPrice, string vehicleNumber)
        {
            return new CarToAddCommand()
            {
                Model = model,
                Make = make,
                RentPrice = rentPrice,
                VehicleNumber = vehicleNumber
            };
        }
    }
}