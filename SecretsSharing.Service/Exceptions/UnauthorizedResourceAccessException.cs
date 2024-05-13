using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Exceptions
{
    public class UnauthorizedResourceAccessException : Exception
    {
        public UnauthorizedResourceAccessException() { }

        public UnauthorizedResourceAccessException(string message)
            : base(message) { }

        public UnauthorizedResourceAccessException(string message, Exception inner)
            : base(message, inner) { }
    }
}
