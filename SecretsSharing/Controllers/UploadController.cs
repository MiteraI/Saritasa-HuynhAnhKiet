using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Domain.Entities.Enums;
using SecretsSharing.Dto.Upload;
using SecretsSharing.Service.Exceptions;
using SecretsSharing.Service.Services.Interfaces;
using System.IO;
using System.Security.Claims;

namespace SecretsSharing.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _log;
        private readonly IUploadService _uploadService;

        public UploadController(ILogger<UploadController> log, IUploadService uploadService)
        {
            _log = log;
            _uploadService = uploadService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Upload>>> GetUploadsAsync([FromQuery] int page, [FromQuery] int size)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var uploads = await _uploadService.GetUploadsAsync(userId, page, size);

            return Ok(uploads);
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
                Response.Headers.Append("Content-Disposition", $"attachment; filename={upload.FileName}");
                Response.Headers.Append("Content-Type", "application/octet-stream");

                return File(stream, "application/octet-stream");
            }
            else
            {
                // In case database is manually updated and upload type is invalid
                return BadRequest("Invalid upload type");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Upload>> UploadSecretAsync([FromForm] UploadDto uploadDto)
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
                _log.LogError(ex, $"Error while uploading secret for {userEmail}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAutoDelete([FromRoute] string id, UploadIsAutoDeleteDto isAutoDelete)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            string userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            try
            {
                var updatedUpload = await _uploadService.UpdateAutoDeleteAsync(id, userId, isAutoDelete.IsAutoDelete);

                return Ok(updatedUpload);
            }
            catch (ResourceNotFoundException ex)
            {
                _log.LogInformation($"{ex.Message} to {userEmail}");
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.LogWarning($"{ex.Message} to {userEmail}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error while updating auto delete for {userEmail}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSecretAsync([FromRoute] string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            string userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            try
            {
                await _uploadService.DeleteUploadAsync(id, userId);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                _log.LogInformation($"{ex.Message} to {userEmail}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.LogWarning($"{ex.Message} to {userEmail}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error while deleting secret for {userEmail}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
