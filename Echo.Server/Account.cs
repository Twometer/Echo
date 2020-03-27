using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Server
{
    internal class Account
    {
        public Guid Id { get; set; }

        public string Tag { get; set; }

        public byte[] PasswordHash { get; set; }

    }
}
