using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Repository.Repositories.Interfaces;
using SecretsSharing.Service.Exceptions;
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
        protected readonly IUploadRepository _uploadRepository;
        private readonly string bucketName = "saritasa";
        public UploadService(IAmazonS3 s3Client, IUploadRepository uploadRepository)
        {
            _s3Client = s3Client;
            _uploadRepository = uploadRepository;
        }

        public async Task DeleteUploadAsync(string secretId, string userId)
        {
            Upload upload = await _uploadRepository.GetOneAsync(secretId);
            if (upload == null)
            {
                throw new ResourceNotFoundException("The upload doesn't exist");
            }
            if (!upload.UserId.Equals(userId))
            {
                throw new UnauthorizedAccessException("The upload doesn't belong to you");
            }

            await _uploadRepository.DeleteSecretByIdAsync(secretId);
        }

        public async Task<Stream> DownloadFileAsync(Upload upload)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = upload.FileName,
            };
            var response = await _s3Client.GetObjectAsync(request);

            // Delete the file on S3 as well if the upload is set to auto delete
            if (upload.IsAutoDelete)
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = upload.FileName
                };
                await _s3Client.DeleteObjectAsync(deleteRequest);
            }

            return response.ResponseStream;
        }

        public async Task<Upload?> GetUploadAsync(string secretId)
        {
            var upload = await _uploadRepository.GetOneAsync(secretId);
            if (upload == null)
            {
                return null;
            }

            if (upload.IsAutoDelete)
            {
                await _uploadRepository.DeleteSecretAsync(upload);
            }

            return upload;
        }

        public Task<IEnumerable<Upload>> GetUploadsAsync(string userId, int position, int size)
        {
            return _uploadRepository.QueryHelper()
                .Filter(x => x.UserId == userId)
                .GetPaginAsync(position, size);
        }

        public async Task<Upload> UpdateAutoDeleteAsync(string secretId, string userId, bool isAutoDelete)
        {
            Upload upload = await _uploadRepository.GetOneAsync(secretId);
            if (upload == null)
            {
                throw new ResourceNotFoundException("The upload doesn't exist");
            }
            if (!upload.UserId.Equals(userId))
            {
                throw new UnauthorizedAccessException("The upload doesn't belong to you");
            }

            upload.IsAutoDelete = isAutoDelete;
            upload.LastModifiedDate = DateTime.Now;
            await _uploadRepository.SaveChangesAsync();

            return upload;
        }

        public async Task<Upload> UploadFileAsync(MemoryStream stream, Upload upload)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = upload.FileName,
                InputStream = stream
            };
            await fileTransferUtility.UploadAsync(uploadRequest);

            Upload createdUpload = _uploadRepository.Add(upload);
            await _uploadRepository.SaveChangesAsync();

            return createdUpload;
        }

        public async Task<Upload> UploadMessageAsync(Upload upload)
        {
            Upload createdUpload = _uploadRepository.Add(upload);
            await _uploadRepository.SaveChangesAsync();

            return createdUpload;
        }
    }
}
