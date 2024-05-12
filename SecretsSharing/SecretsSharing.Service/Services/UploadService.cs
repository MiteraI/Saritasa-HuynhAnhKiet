using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Services
{
    public class UploadService : IUploadService
    {
        protected readonly IAmazonS3 _s3Client;
        private readonly string bucketName = "saritasa";
        public UploadService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<Stream> DownloadFileAsync(string fileKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey,
            };

            var response = await _s3Client.GetObjectAsync(request);

            return response.ResponseStream;
        }

        public Task<Upload> GetUploadAsync(string secretId)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFileAsync(MemoryStream stream, string fileName)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream
            };

            await fileTransferUtility.UploadAsync(uploadRequest);
        }

        public Task UploadMessageAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
