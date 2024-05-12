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
    }
}
