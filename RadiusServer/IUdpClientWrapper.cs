using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public interface IUdpClientWrapper
    {
        Socket Client { get; }

        void Close();
        void Dispose();
        void Send(byte[] content, int length, IPEndPoint recipient);
        Task<UdpReceiveResult> ReceiveAsync();
    }
}