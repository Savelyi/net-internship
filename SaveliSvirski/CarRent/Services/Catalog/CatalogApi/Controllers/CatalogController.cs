using BusinessLogic.Dto;
using CQRS.Commands;
using CQRS.Queries;
using Data.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR;

namespace CatalogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator mediator;

        public CatalogController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        /// <summary> 
        /// Get all cars
        /// </summary>
        /// <param name="requestParameters"></param>
        /// <param name="makeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User got a list of cars</response>
        /// <response code="404">Make was not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCarsAsync([FromQuery] RequestParameters requestParameters,
            [FromQuery] string makeId, CancellationToken cancellationToken)
        {
            var carsToShowDto = await mediator.Send(new GetCarsQuery(requestParameters, makeId), cancellationToken);
            return Ok(carsToShowDto);
        }

        /// <summary>
        /// Get car bu id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">User got a car</response>
        /// <response code="404">Car was not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCarByIdAsync([FromRoute] Guid Id, CancellationToken cancellationToken)
        {
            var carToShowDto = await mediator.Send(new GetCarByIdQuery(Id));
            return Ok(carToShowDto);
        }

        /// <summary>
        /// Create car
        /// </summary>
        /// <param name="carToAdd"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="201">User created a car</response>
        /// <response code="422">Validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCarAsync([FromBody] CarToAddCommand carToAdd,
            CancellationToken cancellationToken)
        {
            var id = await mediator.Send(carToAdd, cancellationToken);
            return StatusCode(201, id);
        }

        /// <summary>
        /// Update car info (for admin)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="carToUpdate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Car was updated</response>
        /// <response code="422">Validation error</response>
        /// <response code="404">Car was not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">No access</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{Id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCarByIdAsync([FromRoute] Guid Id, [FromBody] CarToUpdateDto carToUpdate,
            CancellationToken cancellationToken)
        {
            var carToUpdateCommand = new CarToUpdateCommand(carToUpdate)
            {
                Id = Id
            };
            var id = await mediator.Send(carToUpdateCommand, cancellationToken);
            return Ok(id);
        }

        /// <summary>
        /// Delete car (for admin)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Car was deleted</response>
        /// <response code="400">Car rented, so you cant delete it</response>
        /// <response code="404">Car was not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">No access</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{Id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCarByIdAsync([FromRoute] Guid Id, CancellationToken cancellationToken)
        {
            await mediator.Send(new CarToDeleteCommand(Id), cancellationToken);
            return Ok();
        }
    }
}