using System;
using System.IO;
using System.Net;
using Flexinets.Net;
using Flexinets.Radius;
using Flexinets.Radius.Core;
using Microsoft.Extensions.Logging.Abstractions;

namespace RadiusServerConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "/Content/radius.dictionary";
            var dictionary = new RadiusDictionary(path, NullLogger<RadiusDictionary>.Instance);
            var radiusPacketParser = new RadiusPacketParser(NullLogger<RadiusPacketParser>.Instance, dictionary);

            var packetHandler = new TestPacketHandler();
            var repository = new PacketHandlerRepository();
            repository.AddPacketHandler(IPAddress.Any, packetHandler, "secret");

            var authenticationServer = new RadiusServer(
                new UdpClientFactory(), 
                new IPEndPoint(IPAddress.Any, 1812), 
                radiusPacketParser,
                RadiusServerType.Authentication, repository, NullLogger<RadiusServer>.Instance);

            authenticationServer.Start();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
