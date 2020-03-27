using Echo.Network.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Model
{
    public class ServerInfo
    {
        public string ServerName { get; set; }

        public IEnumerable<User> Users { get; set; } = new List<User>();

        public IEnumerable<Channel> Channels { get; set; } = new List<Channel>();
    }
}
