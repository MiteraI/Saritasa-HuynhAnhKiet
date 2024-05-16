﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Exceptions
{
    public class EmailAlreadyUsedException : Exception
    {
        public EmailAlreadyUsedException(string email) : base($"Email {email} already exist")
        {
        }
    }
}
