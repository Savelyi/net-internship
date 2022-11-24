using BusinessLogic.Dto;
using FluentValidation.TestHelper;
using Validators;
using Xunit;

namespace IdentityApi.Tests
{
    public class ValidatorTests
    {
        private readonly UserToSignUpValidator validator;

        public ValidatorTests()
        {
            validator = new UserToSignUpValidator();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("testUserName", null, "TestUserEmail@gmail.com")]
        [InlineData(null, "testUserPassword", "TestUserEmail@gmail.com")]
        [InlineData("testUserName", "testUserPassword", null)]
        [InlineData("", "", null)]
        public void ValidateUserDto_ShouldFail_WhenAnyOfTheFieldIsNullOrEmpty(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldHaveAnyValidationError().WithErrorCode("NotEmptyValidator");
        }


        [Theory]
        [InlineData("012345678901234567891", "CorrectPass", "CorrectEmail@gmail.com")]
        [InlineData("TooLongUserNameToValidate", "CorrectPass", "CorrectEmail@gmail.com")]
        public void ValidateUserDto_ShouldFail_WhenUserNameIsMoreThan20Char(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);
            var maxLength = 20;

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert

            result.ShouldHaveValidationErrorFor(u => u.UserName).WithErrorCode("MaximumLengthValidator");
        }

        [Theory]
        [InlineData("CorrectUser", "CorrectPass", "CorrectEmail@gmail.com")]
        public void ValidateUserDto_ShouldNotFail_WhenUserNameIsLessThan20Char(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("CorrectUserName", "pass", "CorrectEmail@gmail.com")]
        [InlineData("CorrectUserName", "123p", "CorrectEmail@gmail.com")]
        public void ValidateUserDto_ShouldFail_WhenPasswordIsLessThan6Char(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldHaveValidationErrorFor(u => u.Password).WithErrorCode("MinimumLengthValidator");
        }

        [Theory]
        [InlineData("CorrectUserName", "CorrectPass", "CorrectEmail@gmail.com")]
        public void ValidateUserDto_ShouldNotFail_WhenPasswordIsMoreThan6Char(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("CorrectUserName", "CorrectPass", "CorrectUser@gmail.com")]
        public void ValidateUserDto_ShouldNotFail_WhenEmailMatchesPattern(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("CorrectUserName", "CorrectPass", "IncorrectUser")]
        [InlineData("CorrectUserName", "CorrectPass", "Incorrectusergmail.com")]
        public void ValidateUserDto_ShouldFail_WhenEmailDoesNotMatchPattern(string userName, string password,
            string email)
        {
            // Arrange
            var userToSignUpDtoTest = GetTestUser(userName, password, email);

            // Act
            var result = validator.TestValidate(userToSignUpDtoTest);

            // Assert
            result.ShouldHaveValidationErrorFor(u => u.Email).WithErrorCode("EmailValidator");
        }

        private UserToSignUpDto GetTestUser(string userName, string pass, string email)
        {
            return new UserToSignUpDto()
            {
                UserName = userName,
                Password = pass,
                Email = email
            };
        }
    }
}