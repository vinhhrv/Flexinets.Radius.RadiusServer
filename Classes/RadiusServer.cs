using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RadiusServer));
        private readonly UdpClient _server;
        private readonly RadiusDictionary _dictionary;
        private readonly Dictionary<IPAddress, IPacketHandler> _packetHandlers = new Dictionary<IPAddress, IPacketHandler>();

        public Boolean Running
        {
            get;
            private set;
        }


        /// <summary>
        /// Create a new server on endpoint
        /// </summary>
        /// <param name="serverEndpoint"></param>
        /// <param name="dictionary"></param>
        public RadiusServer(IPEndPoint serverEndpoint, RadiusDictionary dictionary)
        {
            _server = new UdpClient(serverEndpoint);
            _dictionary = dictionary;
        }


        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteEndpoint"></param>
        /// <param name="packethandler"></param>
        public void AddPacketHandler(IPAddress remoteEndpoint, IPacketHandler packethandler)
        {
            _packetHandlers.Add(remoteEndpoint, packethandler);
        }


        /// <summary>
        /// Start listening for requests
        /// </summary>
        public void Start()
        {
            var endpoint = (IPEndPoint)_server.Client.LocalEndPoint;
            _log.InfoFormat("Starting Radius server on {0}:{1}", endpoint.Address, endpoint.Port);

            Running = true;
            _server.BeginReceive(new AsyncCallback(ReceiveCallback), null);

            _log.Info("Server started");
        }


        /// <summary>
        /// Stop listening
        /// </summary>
        public void Stop()
        {
            _log.Info("Stopping server");

            Running = false;
            _server.Close();

            _log.Info("Stopped");
        }


        /// <summary>
        /// Receive packets
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (Running)
            {
                // Immediately start _listening for the next packet
                _server.BeginReceive(new AsyncCallback(ReceiveCallback), null);

                try
                {
                    var sender = new IPEndPoint(IPAddress.Any, 0);
                    var packetbytes = _server.EndReceive(ar, ref sender);
                    _log.DebugFormat("Received packet from {0}:{1}", sender.Address, sender.Port);

                    if (!_packetHandlers.ContainsKey(sender.Address))
                    {
                        _log.ErrorFormat("No packet handler found for remote ip {0}", sender.Address);
                        return;
                    }

                    var handler = _packetHandlers[sender.Address];
                    _log.DebugFormat("Handling packet for remote ip {1} with {0}", handler.GetType(), sender.Address);

                    HandlePacket(handler, packetbytes, sender);
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to receive packet", ex);
                }
            }
        }


        /// <summary>
        ///  Process packets asynchronously
        /// </summary>
        /// <param name="packetbytes"></param>
        /// <param name="sender"></param>
        private void HandlePacket(IPacketHandler packethandler, Byte[] packetbytes, IPEndPoint sender)
        {
            try
            {
                var packet = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.ASCII.GetBytes(packethandler.SharedSecret));
                _log.InfoFormat("Received {0} from {1}:{2} Id={3}", packet.Code, sender.Address, sender.Port, packet.Identifier);

                if (_log.IsDebugEnabled)
                {
                    DumpPacket(packet);
                }

                var sw = new Stopwatch();
                sw.Start();
                var responsepacket = packethandler.HandlePacket(packet);
                sw.Stop();

                _log.DebugFormat("{0}:{1} Id={2}, Received {3} from handler in {4}ms", sender.Address, sender.Port, responsepacket.Identifier, responsepacket.Code, sw.ElapsedMilliseconds);

                using (var client = new UdpClient())
                {
                    var responseBytes = GetBytes(responsepacket);
                    client.Send(responseBytes, responseBytes.Length, sender);
                }

                _log.InfoFormat("{0} sent to {1}:{2} Id={3}", responsepacket.Code, sender.Address, sender.Port, responsepacket.Identifier);
            }
            catch (Exception ex)
            {
                _log.Error("Could not handle packet", ex);
            }
        }


        /// <summary>
        /// Dump the packet attributes to the _log
        /// </summary>
        /// <param name="packet"></param>
        private void DumpPacket(IRadiusPacket packet)
        {
            foreach (var attribute in packet.Attributes)
            {
                if (attribute.Key == "User-Password")
                {
                    _log.Debug(attribute.Key + " length : " + attribute.Value.ToString().Length);
                }
                else
                {
                    _log.DebugFormat("{0} : {1} [{2}]", attribute.Key, attribute.Value, attribute.Value.GetType());
                }
            }
        }


        /// <summary>
        /// Get the raw packet bytes
        /// </summary>
        /// <returns></returns>
        private Byte[] GetBytes(IRadiusPacket packet)
        {
            var packetbytes = new Byte[20]; // Should be 20 + AVPs...
            packetbytes[0] = (Byte)packet.Code;
            packetbytes[1] = packet.Identifier;


            foreach (var attribute in packet.Attributes)
            {
                var contentBytes = new Byte[0];
                if (attribute.Value.GetType() == typeof(String))
                {
                    contentBytes = Encoding.Default.GetBytes((String)attribute.Value);
                }
                else if (attribute.Value.GetType() == typeof(UInt32))
                {
                    contentBytes = BitConverter.GetBytes((UInt32)attribute.Value);
                    Array.Reverse(contentBytes);
                }
                else
                {
                    throw new NotImplementedException();
                }

                var attributeLength = (Byte)(2 + contentBytes.Length);

                // try a normal attribute
                var attributeType = _dictionary.Attributes.SingleOrDefault(o => o.Value.Name == attribute.Key);

                var attributeBytes = new Byte[attributeLength];
                attributeBytes[0] = attributeType.Value.Value;
                attributeBytes[1] = attributeLength;
                Buffer.BlockCopy(contentBytes, 0, attributeBytes, 2, contentBytes.Length);


                Array.Resize(ref packetbytes, packetbytes.Length + attributeBytes.Length);
                Buffer.BlockCopy(attributeBytes, 0, packetbytes, packetbytes.Length - attributeBytes.Length, attributeBytes.Length);
            }


            // Note the order of the bytes...
            var responselengthbytes = BitConverter.GetBytes(packetbytes.Length);
            packetbytes[2] = responselengthbytes[1];
            packetbytes[3] = responselengthbytes[0];

            // Done last... must include packet length
            var responseAuthenticator = CreateResponseAuthenticator(packet.Code, packet.SharedSecret, packet.Identifier, packet.Authenticator, (Int16)packetbytes.Length);
            Buffer.BlockCopy(responseAuthenticator, 0, packetbytes, 4, 16);

            return packetbytes;
        }


        /// <summary>
        /// Creates a response authenticator
        /// Response authenticator = MD5(Code+ID+Length+RequestAuth+Attributes+Secret)
        /// </summary>
        /// <param name="responseCode">The response code to send. This must much the actual response code sent, otherwise the authenticator will be invalid</param>
        /// <returns>Valid response authenticator for the packet</returns>
        private Byte[] CreateResponseAuthenticator(PacketCode responseCode, Byte[] radiusSharedSecret, Byte identifier, Byte[] authenticator, Int16 responselength)
        {
            var responseAuthenticator = new Byte[20 + radiusSharedSecret.Length];
            responseAuthenticator[0] = (Byte)responseCode;
            responseAuthenticator[1] = identifier;

            var responselengthbytes = BitConverter.GetBytes(responselength);

            responseAuthenticator[2] = responselengthbytes[1];
            responseAuthenticator[3] = responselengthbytes[0];
            Buffer.BlockCopy(authenticator, 0, responseAuthenticator, 4, 16);
            Buffer.BlockCopy(radiusSharedSecret, 0, responseAuthenticator, 20, radiusSharedSecret.Length);

            using (var x = MD5CryptoServiceProvider.Create())
            {
                return x.ComputeHash(responseAuthenticator);
            }
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}