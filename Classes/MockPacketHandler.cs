using System;
using System.Net;

namespace Flexinets.Radius
{
    /// <summary>
    /// Creates a response packet according to example in https://tools.ietf.org/html/rfc2865
    /// </summary>
    public sealed class MockPacketHandler : IPacketHandler
    {
        public IRadiusPacket HandlePacket(IRadiusPacket packet)
        {
            if (packet.Code == PacketCode.AccessRequest)
            {
                if (packet.GetAttribute<String>("User-Password") == "arctangent")
                {
                    var responsepacket = packet.CreateResponsePacket(PacketCode.AccessAccept);
                    responsepacket.AddAttribute("Service-Type", (UInt32)1);
                    responsepacket.AddAttribute("Login-Service", (UInt32)0);
                    responsepacket.AddAttribute("Login-IP-Host", IPAddress.Parse("192.168.1.3"));
                    return responsepacket;
                }
            }

            throw new InvalidOperationException("Couldnt handle request?!");
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}