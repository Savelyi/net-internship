using Data.Contracts;
using Data.Models;
using Data.RentContext;

namespace Data.Repository
{
    public class RentRepository : RepositoryBase<Rent>, IRentRepository
    {
        public RentRepository(RentDbContext context)
            : base(context)
        {
        }
    }
}