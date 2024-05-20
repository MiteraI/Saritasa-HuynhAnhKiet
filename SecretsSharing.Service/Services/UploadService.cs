using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Repository.Repositories.Interfaces;
using SecretsSharing.Service.Exceptions;
using SecretsSharing.Service.Services.Interfaces;

namespace SecretsSharing.Service.Services
{
    public class UploadService : IUploadService
    {
        protected readonly IAmazonS3 _s3Client;
        protected readonly IUploadRepository _uploadRepository;
        private string bucketName = "saritasa";
        public UploadService(IAmazonS3 s3Client, IUploadRepository uploadRepository, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _uploadRepository = uploadRepository;

            // Get bucket name from appsettings.json or environment variables
            if (!string.IsNullOrEmpty(configuration.GetValue<string>("BUCKET_NAME")))
            {
                bucketName = configuration.GetValue<string>("BUCKET_NAME")!;
            }
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
            string[] fileNameAndVersionId = upload.FileName.Split(":");
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileNameAndVersionId[0],
                VersionId = fileNameAndVersionId[1]
            };

            // Try-catch if download from S3 fails
            try
            {
                var response = await _s3Client.GetObjectAsync(request);

                // Delete the file on S3 as well if the upload is set to auto delete
                if (upload.IsAutoDelete)
                {
                    // Split filename and versionId
                    var deleteRequest = new DeleteObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileNameAndVersionId[0],
                        VersionId = fileNameAndVersionId[1]
                    };
                    // Only delete in database if the file is successfully deleted in S3
                    await _s3Client.DeleteObjectAsync(deleteRequest);
                    await _uploadRepository.DeleteSecretAsync(upload);
                }

                return response.ResponseStream;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to download file", e);
            }
        }

        public async Task<string> GetMessageAsync(Upload upload)
        {
            if (upload.IsAutoDelete)
            {
                await _uploadRepository.DeleteSecretAsync(upload);
            }
            return upload.MessageText!;
        }

        public async Task<Upload> GetUploadAsync(string secretId)
        {
            var upload = await _uploadRepository.GetOneAsync(secretId);
            if (upload == null)
            {
                return null;
            }

            return upload;
        }

        public Task<IEnumerable<Upload>> GetUploadsAsync(string userId, int page, int size)
        {
            if (page < 1) page = 1;
            return _uploadRepository.QueryHelper()
                .Filter(x => x.UserId == userId)
                .GetPagingAsync((page - 1) * size, size);
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
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = upload.FileName,
                InputStream = stream,
            };


            // Try-catch if upload to S3 fails
            try
            {
                // Get back the file key from S3 after upload since S3 is configured to rename the file
                var response = await _s3Client.PutObjectAsync(putObjectRequest);
                string versionId = response.VersionId;
                // Join filename with versionId
                upload.FileName = $"{upload.FileName}:{versionId}";
            }
            catch (Exception e)
            {
                throw new Exception("Failed to upload file", e);
            }

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
