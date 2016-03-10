using System;

namespace Flexinets.Radius
{
    public class TestPacketHandler : IPacketHandler
    {
        public IRadiusPacket HandlePacket(IRadiusPacket packet)
        {
            // Simulate lag
            //Thread.Sleep(new Random().Next(100, 3000));

            if (packet.Code == PacketCode.AccountingRequest)
            {
                var acctStatusType = packet.GetAttribute<AcctStatusTypes>("Acct-Status-Type");
                if (acctStatusType == AcctStatusTypes.Start)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }

                if (acctStatusType == AcctStatusTypes.Stop)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }

                if (acctStatusType == AcctStatusTypes.InterimUpdate)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }
            }
            else if (packet.Code == PacketCode.AccessRequest)
            {
                if (packet.GetAttribute<String>("User-Name") == "user@example.com" && packet.GetAttribute<String>("User-Password") == "1234")
                {
                    var response = packet.CreateResponsePacket(PacketCode.AccessAccept);
                    response.AddAttribute("Acct-Interim-Interval", 60);
                    return response;
                }
                return packet.CreateResponsePacket(PacketCode.AccessReject);
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