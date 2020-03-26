using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Echo.Server
{
    internal class Storage
    {
        public static IDictionary<string, Account> Accounts { get; } = new ConcurrentDictionary<string, Account>();

        

    }
}
