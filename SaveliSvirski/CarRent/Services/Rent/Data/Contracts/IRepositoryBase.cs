using System.Linq.Expressions;
using Data.Models;

namespace Data.Contracts
{
    public interface IRepositoryBase<T> where T : BaseModel
    {
        IQueryable<T> GetAll(bool trackChanges);

        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression,
            bool trackChanges = false);

        Task<T> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default, bool trackChanges = false);
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
    }
}