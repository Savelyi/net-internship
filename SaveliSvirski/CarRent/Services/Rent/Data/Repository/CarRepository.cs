using Data.Contracts;
using Data.Models;
using Data.RentContext;

namespace Data.Repository
{
    public class CarRepository : RepositoryBase<Car>, ICarRepository
    {
        public CarRepository(RentDbContext context)
            : base(context)
        {
        }
    }
}