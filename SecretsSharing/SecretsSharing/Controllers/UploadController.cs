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

        [HttpGet("view/{secretId}")]
        public async Task<IActionResult> ViewSecretAsync([FromRoute] string secretId)
        {
            var upload = await _uploadService.GetUploadAsync(secretId);

            var stream = await _uploadService.DownloadFileAsync(secretId);
            Response.Headers.Add("Content-Disposition", $"attachment; filename={secretId}");

            return File(stream, "application/octet-stream");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadSecretAsync([FromForm] UploadDto uploadDto)
        {
            //Get subject as Id
            var subject = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
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
