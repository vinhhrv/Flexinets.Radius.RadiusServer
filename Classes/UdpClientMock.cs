using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public class foo : IAsyncResult
    {
        public object AsyncState
        {
            get
            {
                throw new NotImplementedException("yay");
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCompleted
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }


    public class UdpClientMock : IUdpClientWrapper
    {
        private UdpClient _client;
        private Boolean sent = false;

        public UdpClientMock(IPEndPoint endpoint)
        {
            _client = new UdpClient(endpoint);
        }

        public Socket Client
        {
            get
            {
                return _client.Client;
            }
        }

        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
        {
            var foo = new foo();
            if (!sent)
            {
                sent = true;
                requestCallback.Invoke(foo);
            }
            return foo;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
        {
            // var sender = new IPEndPoint(IPAddress.Any, 0);
            // var packetbytes = _server.EndReceive(ar, ref sender);

            remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50000);

            var request = "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003";
            var bytes = Utils.StringToByteArray(request);
            return bytes;
        }

        public Task<UdpReceiveResult> ReceiveAsync()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] content, int length, IPEndPoint recipient)
        {
            throw new NotImplementedException();
        }
    }
}
