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
            if (upload == null)
            {
                return NotFound();
            }

            if (upload.UploadType == UploadTypes.MESSAGE)
            {
                return Ok(upload.MessageText);
            }
            else if (upload.UploadType == UploadTypes.FILE)
            {
                var stream = await _uploadService.DownloadFileAsync(upload);
                Response.Headers.Append("Content-Disposition", $"attachment; filename={id}");
                Response.Headers.Append("Content-Type", "application/octet-stream");

                return File(stream, "application/octet-stream");
            } else
            {
                // In case database is manually updated and upload type is invalid
                return BadRequest("Invalid upload type");
            }
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
                        upload.UploadType = UploadTypes.FILE;
                        await uploadDto.File.CopyToAsync(memoryStream);
                        upload = await _uploadService.UploadFileAsync(memoryStream, upload);
                    }

                    // Add resource URL to the response header
                    return CreatedAtAction(nameof(ViewSecretAsync), new { id = upload.Id }, upload);
                }
                else
                {
                    upload.MessageText = uploadDto.MessageText;
                    upload.UploadType = UploadTypes.MESSAGE;
                    upload = await _uploadService.UploadMessageAsync(upload);

                    // Add resource URL to the response header
                    return CreatedAtAction(nameof(ViewSecretAsync), new { id = upload.Id }, upload);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAutoDelete([FromRoute] string id, UploadIsAutoDeleteDto isAutoDelete)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            try
            {
                var updatedUpload = await _uploadService.UpdateAutoDeleteAsync(id, userId, isAutoDelete.IsAutoDelete);
                if (updatedUpload == null)
                {
                    return NotFound("The upload doesn't exist or not belong to you");
                }

                return Ok(updatedUpload);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
