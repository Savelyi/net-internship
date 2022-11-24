using BusinessLogic.Contracts;
using BusinessLogic.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ILogger<AccountController> logger;

        public AccountController(ILogger<AccountController> _logger, IAccountService _accountService)
        {
            logger = _logger;
            accountService = _accountService;
        }

        /// <summary>
        /// Sign Up
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <response code="201">User created</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("SignUp")]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SignUp([FromBody] UserToSignUpDto userDto)
        {
            await accountService.SignUpAsync(userDto);
            return StatusCode(201);
        }
    }
}