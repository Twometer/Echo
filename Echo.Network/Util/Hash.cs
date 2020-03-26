using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Echo.Network.Util
{
    public class Hash
    {

        public static byte[] Sha256(string data)
        {
            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

    }
}
