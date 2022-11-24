using BusinessLogic.Dto;

namespace BusinessLogic.Contracts
{
    public interface IAccountService
    {
        public Task SignUpAsync(UserToSignUpDto userDto);
    }
}