using System.Linq.Expressions;
using Data.CatalogContext;
using Data.Contracts;
using Data.Models;
using Data.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseModel
    {
        protected CatalogDbContext context;

        public RepositoryBase(CatalogDbContext context)
        {
            this.context = context;
        }

        public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public virtual void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public virtual async Task<PagedList<T>> GetAllAsync(RequestParameters requestParameters,
            CancellationToken cancellationToken, bool trackChanges = false)
        {
            var result = trackChanges
                ? context.Set<T>()
                : context.Set<T>().AsNoTracking();
            return await PagedList<T>.ToPagedListAsync(result, requestParameters.PageNumber, requestParameters.PageSize);
        }

        public virtual IQueryable<T> GetAll(CancellationToken cancellationToken = default, bool trackChanges = false)
        {
            return trackChanges
                ? context.Set<T>()
                : context.Set<T>().AsNoTracking();
        }

        public virtual async Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression, 
            CancellationToken cancellationToken, bool trackChanges)
        {
            return trackChanges
                ? await context.Set<T>().FirstOrDefaultAsync(expression, cancellationToken: cancellationToken)
                : await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);
        }

        public virtual IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken, bool trackChanges = false)
        {
            return trackChanges
                ? context.Set<T>().Where(expression)
                : context.Set<T>().AsNoTracking().Where(expression);
        }

        public async Task<T> GetByIdAsync(Guid? Id, CancellationToken cancellationToken = default, bool trackChanges = false)
        {
            return trackChanges
                ? await context.Set<T>().FirstOrDefaultAsync(e => e.Id == Id, cancellationToken)
                : await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == Id, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
    }
}