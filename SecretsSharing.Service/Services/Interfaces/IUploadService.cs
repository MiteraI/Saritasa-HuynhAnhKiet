﻿using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Services.Interfaces
{
    public interface IUploadService
    {
        Task<Upload> UploadFileAsync(MemoryStream stream, Upload upload);
        Task<Upload> UploadMessageAsync(Upload upload);
        Task<Stream> DownloadFileAsync(Upload upload);
        Task<string> GetMessageAsync(Upload upload);
        Task<Upload> GetUploadAsync(string secretId);
        Task<IEnumerable<Upload>> GetUploadsAsync(string userId, int page, int size);
        Task<Upload> UpdateAutoDeleteAsync(string secretId, string userId, bool isAutoDelete);
        Task DeleteUploadAsync(string secretId, string userId);
    }
}
