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
        public string? MessageText { get; set; }
        public bool? IsAutoDelete { get; set; } = false;
        public IFormFile? File { get; set; }
    }
}
