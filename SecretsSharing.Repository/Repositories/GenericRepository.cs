﻿using Microsoft.EntityFrameworkCore;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>, IDisposable where TEntity : BaseEntity<TKey>
    {
        protected internal readonly ApplicationDatabaseContext _context;
        protected internal readonly DbSet<TEntity> _dbSet;

        //Even though there is only one other repository, it is better to have a generic repository to avoid code duplication and extend codebase easily in the future.
        public GenericRepository(ApplicationDatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.FromResult(_dbSet.Remove(entity));
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            var entity = await GetOneAsync(id);
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetOneAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IFluentRepository<TEntity> QueryHelper()
        {
            var repository = new FluentRepository<TEntity>(_dbSet);
            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public TEntity Update(TEntity entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IEnumerable<TEntity>> GetPagingAsync(int position, int size)
        {
            return await _dbSet.Skip(position).Take(size).ToListAsync();
        }
    }
}
