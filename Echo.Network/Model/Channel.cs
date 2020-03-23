using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Model
{
    public class Channel
    {
        public Guid ChannelId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ChannelType Type { get; set; }


        public enum ChannelType
        {
            Voice,
            Text
        }
    }
}
