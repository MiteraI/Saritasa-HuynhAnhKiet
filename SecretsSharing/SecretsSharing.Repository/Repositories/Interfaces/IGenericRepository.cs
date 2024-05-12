using Azure;
using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Repository.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<TEntity> GetOneAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task DeleteByIdAsync(TKey id);
        Task DeleteAsync(TEntity entity);
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        Task<int> SaveChangesAsync();
        IFluentRepository<TEntity> QueryHelper();
    }
}
