using Microsoft.AspNetCore.Http;
using SecretsSharing.Domain.Entities.Enums;
using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Dto.Upload
{
    public class UploadDto
    {
        public string? Id { get; set; }
        public UploadTypes? UploadType { get; set; }
        public string? FileName { get; set; }
        public string? MessageText { get; set; }
        public bool? IsAutoDelete { get; set; } = false;
        public string? UserId { get; set; }
        public User? User { get; set; } = null;
        public IFormFile? File { get; set; }
    }
}
