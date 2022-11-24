using System.Linq.Expressions;
using Data.Models;
using Data.RequestFeatures;

namespace Data.Contracts
{
    public interface IRepositoryBase<T> where T : BaseModel
    {
        Task<PagedList<T>> GetAllAsync(RequestParameters requestParameters, CancellationToken cancellationToken,
            bool trackChanges = false);

        IQueryable<T> GetAll(CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken=default, bool trackChanges = false);

        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression, CancellationToken cancellationToken,
            bool trackChanges = false);

        Task<T> GetByIdAsync(Guid? Id, CancellationToken cancellationToken = default, bool trackChanges = false);
        Task CreateAsync(T entity, CancellationToken cancellationToken);
        void Update(T entity);
        void Delete(T entity);
    }
}