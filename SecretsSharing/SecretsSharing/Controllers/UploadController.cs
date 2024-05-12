using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Domain.Entities.Enums;
using SecretsSharing.Dto.Upload;
using SecretsSharing.Service.Services.Interfaces;
using System.IO;
using System.Security.Claims;

namespace SecretsSharing.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewSecretAsync([FromRoute] string id)
        {
            var upload = await _uploadService.GetUploadAsync(id);

            var stream = await _uploadService.DownloadFileAsync(id);
            Response.Headers.Add("Content-Disposition", $"attachment; filename={id}");

            return File(stream, "application/octet-stream");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadSecretAsync([FromForm] UploadDto uploadDto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            Upload upload = new Upload
            {
                UserId = userId,
                IsAutoDelete = uploadDto.IsAutoDelete ?? false,
                CreatedBy = userEmail,
                CreatedDate = DateTime.Now,
            };

            try
            {
                if (uploadDto.File != null && uploadDto.File?.Length != 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        upload.FileName = uploadDto.File.FileName;
                        await uploadDto.File.CopyToAsync(memoryStream);
                        upload = await _uploadService.UploadFileAsync(memoryStream, upload);
                    }

                    return Created(upload.Id, upload);
                }
                else
                {
                    upload.MessageText = uploadDto.MessageText;
                    upload.UploadType = UploadTypes.MESSAGE;
                    upload = await _uploadService.UploadMessageAsync(upload);

                    return Created(upload.Id, upload);
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
