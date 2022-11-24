using BusinessLogic.Contracts;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentApi.Constants;
using SharedModels.Constants;

namespace RentApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    [ApiController]
    public class RentJobsController : ControllerBase
    {
        private readonly IRecurringJobManager recurringJobManager;
        private readonly IRentJobsService rentsJobService;

        public RentJobsController(IRecurringJobManager recurringJobManager, IRentJobsService rentsJobService)
        {
            this.recurringJobManager = recurringJobManager;
            this.rentsJobService = rentsJobService;
        }

        [HttpGet]
        public IActionResult CreateRecurringJob()
        {
            recurringJobManager.AddOrUpdate(JobNamesConstants.DeleteOldRentsJob, () => rentsJobService.DeleteOldRents(), Cron.Weekly);
            return Ok();
        }
    }
}