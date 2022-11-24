using System.Linq.Expressions;
using Data.Contracts;
using Data.Models;
using Data.RentContext;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseModel
    {
        protected RentDbContext context;

        public RepositoryBase(RentDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll(bool trackChanges)
        {
            return trackChanges
                ? context.Set<T>()
                : context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            return trackChanges
                ? context.Set<T>().Where(expression)
                : context.Set<T>().AsNoTracking().Where(expression);
        }

        public async Task<T> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default,
            bool trackChanges = false)
        {
            return trackChanges
                ? await context.Set<T>().FirstOrDefaultAsync(e => e.Id == Id, cancellationToken)
                : await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == Id, cancellationToken);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
    }
}