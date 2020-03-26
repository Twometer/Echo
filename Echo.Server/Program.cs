using Echo.Network;
using Echo.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Echo
{
    public class Program
    {
        private static IDictionary<Guid, Client> clients = new Dictionary<Guid, Client>();

        public static async void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, NetConfig.Port);
            listener.Start();

            while (true)
            {
                var tcp = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Incoming connection from " + tcp.Client.RemoteEndPoint);

                var client = new Client(tcp);
                clients[client.Id] = client;
                client.BeginReading();
            }
        }
    }
}
