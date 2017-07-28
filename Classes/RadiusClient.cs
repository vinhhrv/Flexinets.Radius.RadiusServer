using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public class RadiusClient
    {
        private readonly UdpClient _udpClient;
        private readonly RadiusDictionary _dictionary;
        private readonly List<Task<IRadiusPacket>> _tasks = new List<Task<IRadiusPacket>>();

        public RadiusClient(IPEndPoint localEndpoint, RadiusDictionary dictionary)
        {
            _udpClient = new UdpClient(localEndpoint);
            _dictionary = dictionary;
        }


        public async Task<IRadiusPacket> SendPacket(IRadiusPacket packet, IPEndPoint remoteEndpoint)
        {
            var packetBytes = packet.GetBytes(_dictionary);

            await _udpClient.SendAsync(packetBytes, packetBytes.Length, remoteEndpoint);
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                _udpClient.Close();
            });
            var response = await _udpClient.ReceiveAsync();

            return RadiusPacket.ParseRawPacket(response.Buffer, _dictionary, Encoding.UTF8.GetBytes("secret"));
        }
    }
}
