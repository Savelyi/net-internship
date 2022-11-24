using Data.Contracts;
using Data.RentContext;

namespace Data.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RentDbContext context;
        private IRentRepository rentRepository;
        private ICarRepository carRepository;

        public RepositoryManager(RentDbContext context)
        {
            this.context = context;
        }

        public ICarRepository Cars => carRepository ??= new CarRepository(context);

        public IRentRepository Rents => rentRepository ??= new RentRepository(context);

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}