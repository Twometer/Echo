using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.Network.Model
{
    public class Message
    {
        public Guid MessageId { get; set; }

        public Guid ChannelId { get; set; }

        public Guid SenderId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }
    }
}
