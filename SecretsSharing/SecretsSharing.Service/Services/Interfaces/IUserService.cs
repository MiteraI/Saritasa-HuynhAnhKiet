using SecretsSharing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> Authenticate(string email, string password);
        Task<User> Register(User user, string password);
    }
}
