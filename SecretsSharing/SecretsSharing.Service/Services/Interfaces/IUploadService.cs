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
        Task<Stream> DownloadFileAsync(string fileKey);
        Task<Upload> GetUploadAsync(string secretId);
        Task<Upload> UpdateAutoDeleteAsync(string secretId, bool isAutoDelete);
    }
}
