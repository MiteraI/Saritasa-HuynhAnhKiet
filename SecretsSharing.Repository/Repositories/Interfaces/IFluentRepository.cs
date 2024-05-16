using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Repository.Repositories.Interfaces
{
    public interface IFluentRepository<TEntity> where TEntity : class
    {
        IFluentRepository<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        IFluentRepository<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        Task<TEntity?> GetOneAsync();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetPagingAsync(int position, int size);
        IFluentRepository<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IFluentRepository<TEntity> Include(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include);
        IFluentRepository<TEntity> AsNoTracking();
    }
}
