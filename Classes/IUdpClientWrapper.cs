using System;
using System.Net;
using System.Net.Sockets;

namespace Flexinets.Radius
{
    public interface IUdpClientWrapper
    {
        Socket Client { get; }

        IAsyncResult BeginReceive(AsyncCallback requestCallback, object state);
        void Close();
        void Dispose();
        byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP);
        void Send(byte[] content, int length, IPEndPoint recipient);
    }
}