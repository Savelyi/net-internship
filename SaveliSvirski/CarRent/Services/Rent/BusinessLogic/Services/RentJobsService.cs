using BusinessLogic.Contracts;
using Data.Contracts;
using Microsoft.Extensions.Caching.Distributed;

namespace BusinessLogic.Services
{
    public class RentJobsService : IRentJobsService
    {
        private readonly IRepositoryManager repository;
        private readonly IDistributedCache cache;

        public RentJobsService(IRepositoryManager repository, IDistributedCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }

        public void DeleteOldRents()
        {
            var oldRents = repository.Rents.GetByCondition(e => e.IsClosed, true).ToList()
                .Where(e => DateTime.Now.Subtract((DateTime) e.Closed).TotalDays > 7);

            foreach (var oldRent in oldRents)
            {
                repository.Rents.Delete(oldRent);
                cache.Remove(oldRent.UserId.ToString());
            }

            repository.Save();
        }
    }
}