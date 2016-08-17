using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public class UdpClientWrapper : IDisposable, IUdpClientWrapper
    {
        private UdpClient _client;
        public Socket Client
        {
            get { return _client.Client; }
        }

        public UdpClientWrapper(IPEndPoint localEndpoint)
        {
            _client = new UdpClient(localEndpoint);
        }


        public void Close()
        {
            _client.Close();
        }


        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
        {
            return _client.BeginReceive(requestCallback, state);
        }


        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
        {
            return _client.EndReceive(asyncResult, ref remoteEP);
        }


        public void Send(Byte[] content, Int32 length, IPEndPoint recipient)
        {
            _client.Send(content, length, recipient);
        }


        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
