using System;
using System.Collections.Generic;
using System.Net;
using Flexinets.Radius.Core;
using Microsoft.AspNetCore.HttpOverrides;

namespace Flexinets.Radius
{
    public interface IPacketHandlerRepository
    {
        bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler);
    }
}