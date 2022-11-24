using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;
        private readonly IValidator<UserToSignUpDto> validator;
        private readonly UserManager<IdentityUser> userManager;

        public AccountService(IMapper mapper, IValidator<UserToSignUpDto> validator, UserManager<IdentityUser> userManager)
        {
            this.mapper = mapper;
            this.validator = validator;
            this.userManager = userManager;
        }

        public async Task SignUpAsync(UserToSignUpDto userDto)
        {
            await validator.ValidateAndThrowAsync(userDto);
            var user = mapper.Map<IdentityUser>(userDto);
            var result = await userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                var mes = "";
                foreach (var error in result.Errors)
                {
                    mes += error.Description + ";\n";
                }

                throw new ValidationException(mes);
            }

            await userManager.AddToRoleAsync(user, "user");
        }
    }
}