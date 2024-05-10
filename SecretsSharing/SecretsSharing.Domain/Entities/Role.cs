using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Domain.Entities
{   
    public class Role : IdentityRole<string>
    {
        public virtual ICollection<User> Users { get; set; }
    }
}
