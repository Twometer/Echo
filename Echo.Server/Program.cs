﻿using Echo.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Echo
{
    public class Program
    {
        private const int Port = 13373;
        private static IDictionary<Guid, Client> clients = new Dictionary<Guid, Client>();

        public static async void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                var tcp = await listener.AcceptTcpClientAsync();
                var client = new Client(tcp);
                clients[client.Id] = client;
                client.BeginReading();
            }
        }
    }
}
