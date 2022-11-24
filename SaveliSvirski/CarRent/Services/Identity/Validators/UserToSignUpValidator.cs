using BusinessLogic.Dto;
using FluentValidation;

namespace Validators
{
    public class UserToSignUpValidator : UserToManipulateValidator<UserToSignUpDto>
    {
        public UserToSignUpValidator()
            : base()
        {
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email field is required"); 
            RuleFor(u => u.Email).EmailAddress();
        }
    }
}