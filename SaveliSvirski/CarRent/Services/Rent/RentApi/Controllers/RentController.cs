using System.Security.Claims;
using BusinessLogic.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentController : ControllerBase
    {
        private readonly IRentService rentService;

        public RentController(IRentService rentService)
        {
            this.rentService = rentService;
        }

        /// <summary>
        /// Get all user's rents 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User got his rents</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUserRentsAsync(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await rentService.ShowUserRentsAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get user's rent by id
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User got his rent</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rent was not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{rentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserRentByIdAsync([FromRoute] Guid rentId,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await rentService.ShowUserRentAsync(rentId, userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create new rent
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="201">User created rent</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Car was not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRentAsync([FromBody] Guid carId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await rentService.CreateRentAsync(carId, userId, cancellationToken);
            return StatusCode(201, result);
        }

        /// <summary>
        /// Close rent
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User closed his rent</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rent was not found</response>
        /// <response code="400">Rent has been already closed</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{rentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CloseRentAsync([FromRoute] Guid rentId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await rentService.CloseRentAsync(rentId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Delete user's rent
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User deleted his rent</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rent was not found</response>
        /// <response code="400">Rent has not been closed yet</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{rentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRentAsync([FromRoute] Guid rentId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await rentService.DeleteRentAsync(rentId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Delete any user's rent (for admin)
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200"> User's rent deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">No access</response>
        /// <response code="404">Rent was not found</response>
        /// <response code="400">Rent has not been closed yet</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAnyRentForAdminAsync([FromQuery] Guid rentId,
            CancellationToken cancellationToken)
        {
            await rentService.DeleteRentForAdminAsync(rentId, cancellationToken);
            return Ok();
        }

        private Guid GetUserId()
        {
            return Guid.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
    }
}