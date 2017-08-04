using Flexinets.Radius.Core;
using System;

namespace Flexinets.Radius
{
    public interface IPacketHandler : IDisposable
    {
        IRadiusPacket HandlePacket(IRadiusPacket packet);
    }
}
