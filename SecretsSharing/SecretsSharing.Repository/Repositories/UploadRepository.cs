using Microsoft.EntityFrameworkCore;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Repository.Repositories
{
    public class UploadRepository : GenericRepository<Upload, string>, IUploadRepository
    {
        public UploadRepository(ApplicationDatabaseContext context) : base(context)
        {
        }

        public async Task DeleteSecretAsync(Upload upload)
        {
            // Delete is called and the row is locked until the transaction is committed
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    _dbSet.Remove(upload);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task DeleteSecretByIdAsync(string secretId)
        {
            // Delete is called and the row is locked until the transaction is committed 
            var upload = await GetOneAsync(secretId);
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    _dbSet.Remove(upload);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
