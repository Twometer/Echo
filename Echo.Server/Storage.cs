using Echo.Network.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Echo.Server
{
    internal class Storage
    {
        public static Storage Instance { get; private set; } = new Storage();

        public string ServerName { get; set; }

        public IDictionary<string, Account> Accounts { get; } = new ConcurrentDictionary<string, Account>();

        public IDictionary<Guid, Channel> Channels { get; } = new ConcurrentDictionary<Guid, Channel>();

        private static volatile bool saving = false;

        public static void Save()
        {
            if (saving) return;

            saving = true;
            var data = JsonConvert.SerializeObject(Instance);
            File.WriteAllText("./config.json", data, Encoding.UTF8);
            saving = false;
        }

        public static void Load()
        {
            if (!File.Exists("./config.json"))
                return;
            var data = File.ReadAllText("./config.json");
            Instance = JsonConvert.DeserializeObject<Storage>(data);
        }
    }
}
