using CQRS.Commands;
using FluentValidation.TestHelper;
using Validator;
using Xunit;

namespace CatalogApi.Tests.ValidatorTests
{
    public class CarToUpdateValidatorTests
    {
        private readonly CarToUpdateValidator validator;

        public CarToUpdateValidatorTests()
        {
            validator = new CarToUpdateValidator();
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, null)]
        [InlineData("eb6e8309-1d70-424e-9715-3289f97eba2a", "newModel", "newMake", null, "1337HR-7")]
        [InlineData("", "newModel", "", 100, "1337HR-7")]
        public void ValidateCarToUpdateCommand_ShouldFail_WhenAnyOfTheFieldsIsNullOrEmpty(string id, string model,
            string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToUpdateCommand = GetTestCarCommand(id, model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToUpdateCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("NotEmptyValidator");
        }

        [Theory]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337HR-79")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337HR-")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337H")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "13R-7")]
        public void ValidateCarToAddCommand_ShouldFail_WhenVehicleNumberLengthLessOrMoreThan8Characters(string id,
            string model, string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(id, model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("ExactLengthValidator");
        }

        [Theory]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337HRR7")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "13375-HR")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337H-HT")]
        public void ValidateCarToAddCommand_ShouldFail_WhenVehicleNumberDoesNotMatchThePattern(string id, string model,
            string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(id, model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldHaveAnyValidationError().WithErrorCode("RegularExpressionValidator");
        }

        [Theory]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "1337HR-7")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "4567BR-5")]
        [InlineData("08f0adf5-9e68-42b5-a130-d3fe6b09de97", "newModel", "newMake", 100, "8193TA-3")]
        public void ValidateCarToAddCommand_ShouldNotFail_WhenVehicleNumberMatchesThePattern(string id, string model,
            string make,
            decimal rentPrice, string vehicleNumber)
        {
            //Arrange
            var carToAddCommand = GetTestCarCommand(id, model, make, rentPrice, vehicleNumber);

            //Act
            var result = validator.TestValidate(carToAddCommand);

            //Arrange
            result.ShouldNotHaveAnyValidationErrors();
        }

        private CarToUpdateCommand GetTestCarCommand(string id, string model, string make,
            decimal rentPrice, string vehicleNumber)
        {
            Guid.TryParse(id, out var guidId);
            return new CarToUpdateCommand()
            {
                Id = guidId,
                Model = model,
                Make = make,
                RentPrice = rentPrice,
                VehicleNumber = vehicleNumber
            };
        }
    }
}