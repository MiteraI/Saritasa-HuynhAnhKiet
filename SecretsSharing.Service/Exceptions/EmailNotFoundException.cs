using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Exceptions
{
    public class EmailNotFoundException : Exception
    {
        public EmailNotFoundException(string message) : base(message)
        {
        }
    }
}
