using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Server
{
    internal class Account
    {
        public string Tag { get; set; }

        public byte[] PasswordHash { get; set; }

    }
}
