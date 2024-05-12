using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Services.Interfaces
{
    public interface IUploadService
    {
        Task UploadFileAsync(MemoryStream stream, string fileName);
        Task UploadMessageAsync(string message);
        Task<Stream> DownloadFileAsync(string fileKey);
        Task<Upload> GetUploadAsync(string secretId);
    }
}
