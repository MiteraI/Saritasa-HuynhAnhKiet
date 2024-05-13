using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Repository.Repositories.Interfaces
{
    public interface IUploadRepository : IGenericRepository<Upload, string>
    {
        Task DeleteSecretByIdAsync(string secretId);
        Task DeleteSecretAsync(Upload upload);
    }
}
