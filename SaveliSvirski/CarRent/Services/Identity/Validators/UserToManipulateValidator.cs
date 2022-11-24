using BusinessLogic.Dto;
using FluentValidation;

namespace Validators
{
    public class UserToManipulateValidator<T> : AbstractValidator<T>
        where T : UserToManipulateDto
    {
        public UserToManipulateValidator()
        {
            RuleFor(u => u.UserName).NotEmpty().WithMessage("UserName field is required");
            RuleFor(u => u.UserName).MaximumLength(20);
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password field is required");
            RuleFor(u => u.Password).MinimumLength(6).WithMessage("Password cant be less than 6 characters");
        }
    }
}