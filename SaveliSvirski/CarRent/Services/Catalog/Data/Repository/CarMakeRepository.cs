using Data.CatalogContext;
using Data.Contracts;
using Data.Models;

namespace Data.Repository
{
    public class CarMakeRepository : RepositoryBase<CarMake>, ICarMakeRepository
    {
        public CarMakeRepository(CatalogDbContext context) : base(context)
        {
        }
    }
}