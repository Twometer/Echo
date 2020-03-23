using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Model
{
    public class User
    {
        public Guid Id { get; set; }

        public string EchoTag { get; set; }

        public OnlineState State { get; set; }




        public enum OnlineState
        {
            Online,
            Away,
            DoNotDisturb,
            Offline
        }
    }
}
