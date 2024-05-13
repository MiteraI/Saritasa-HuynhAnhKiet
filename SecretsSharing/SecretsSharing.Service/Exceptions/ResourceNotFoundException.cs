using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() { }

        public ResourceNotFoundException(string message)
            : base(message) { }

        public ResourceNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
