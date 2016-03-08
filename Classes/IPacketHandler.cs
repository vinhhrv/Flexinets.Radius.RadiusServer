using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public interface IPacketHandler : IDisposable
    {
        String SharedSecret { get; }
        IRadiusPacket HandlePacket(IRadiusPacket packet);
    }
}
