using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Dto.Upload;
using SecretsSharing.Service.Services.Interfaces;
using System.IO;

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

        [HttpGet("{secretId}")]
        public async Task<IActionResult> GetFilesAsync([FromRoute] string secretId)
            {
            var stream = await _uploadService.DownloadFileAsync(secretId);
            Response.Headers.Add("Content-Disposition", $"attachment; filename={secretId}");

            return File(stream,  "application/octet-stream");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadDto uploadDto)
        {
            if (uploadDto.File == null)
            {
                return BadRequest("File is null");
            }

            if (uploadDto.File.Length == 0)
            {
                return BadRequest("File is empty");
            }

            using (var memoryStream = new MemoryStream())
            {
                await uploadDto.File.CopyToAsync(memoryStream);
                await _uploadService.UploadFileAsync(memoryStream, uploadDto.File.FileName);
            }

            return Created();
        }
    }
}
