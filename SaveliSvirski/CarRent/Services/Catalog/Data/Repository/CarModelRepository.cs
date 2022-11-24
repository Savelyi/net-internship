using Data.CatalogContext;
using Data.Contracts;
using Data.Models;

namespace Data.Repository
{
    public class CarModelRepository : RepositoryBase<CarModel>, ICarModelRepository
    {
        public CarModelRepository(CatalogDbContext context)
            : base(context)
        {
        }
    }
}