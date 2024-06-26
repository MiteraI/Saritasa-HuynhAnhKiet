﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretsSharing.Domain.Entities
{
    public class User : IdentityUser
    {
        [JsonIgnore] public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
