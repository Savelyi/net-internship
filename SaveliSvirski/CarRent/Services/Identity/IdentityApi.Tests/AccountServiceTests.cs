using AutoMapper;
using BusinessLogic.Dto;
using BusinessLogic.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace IdentityApi.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidator<UserToSignUpDto>> validatorMock = new Mock<IValidator<UserToSignUpDto>>();
        private readonly Mock<UserManager<IdentityUser>> userManagerMock;
        private readonly AccountService service;

        public AccountServiceTests()
        {
            userManagerMock = GetUserManagerMock(new List<IdentityUser>()
            {
                new IdentityUser()
                {
                    UserName = "UserNameForTest",
                    Email = "TestEmail@gmail.com"
                }
            });
            service = new AccountService(mapperMock.Object, validatorMock.Object, userManagerMock.Object);
        }

        [Fact]
        public async Task SignUpAsync_ShouldCreateUser_WhenValidationSucced()
        {
            //Arrange
            var userDto = GetTestUserDto();
            
            mapperMock.Setup(m => m.Map<IdentityUser>(It.IsAny<UserToSignUpDto>()))
                .Returns(new IdentityUser()
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email
                });
            
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            //Act
            await service.SignUpAsync(userDto);

            //Assert
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), userDto.Password));
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));
            userManagerMock.VerifyAll();
        }

        [Fact]
        public async Task SignUpAsync_ShouldThrowValidationException_WhenUserCreationFailed()
        {
            var userDto = GetTestUserDto();
            
            mapperMock.Setup(m => m.Map<IdentityUser>(It.IsAny<UserToSignUpDto>()))
                .Returns(new IdentityUser()
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email
                });

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            //Act
            var act = () => service.SignUpAsync(userDto);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(act);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), userDto.Password));
            userManagerMock.VerifyAll();
        }

        private UserToSignUpDto GetTestUserDto()
        {
            return new UserToSignUpDto()
            {
                Email = "testEmail@gmail.com",
                Password = "testPas123",
                UserName = "testUserDto"
            };
        }

        public Mock<UserManager<IdentityUser>> GetUserManagerMock(List<IdentityUser> ls)
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            return mgr;
        }
    }
}