using Data.CatalogContext;
using Data.Contracts;

namespace Data.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly CatalogDbContext context;
        private ICarMakeRepository makeRepository;
        private ICarModelRepository modelRepository;

        public RepositoryManager(CatalogDbContext context)
        {
            this.context = context;
        }

        public ICarMakeRepository Makes => makeRepository ??= new CarMakeRepository(context);

        public ICarModelRepository Models => modelRepository ??= new CarModelRepository(context);

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}