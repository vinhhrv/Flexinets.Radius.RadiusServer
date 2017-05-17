using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RadiusServer));
        private readonly IUdpClientWrapper _server;
        private readonly RadiusDictionary _dictionary;
        private readonly Dictionary<IPAddress, PacketHandlerContainer> _packetHandlers = new Dictionary<IPAddress, PacketHandlerContainer>();

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
            _server = new UdpClientWrapper(serverEndpoint);
            _dictionary = dictionary;
        }


        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteEndpoint"></param>
        /// <param name="secret"></param>
        /// <param name="packethandler"></param>
        public void AddPacketHandler(IPAddress remoteEndpoint, String secret, IPacketHandler packethandler)
        {
            _log.Info($"Adding packet handler of type {packethandler.GetType()} for remote IP {remoteEndpoint.ToString()}");
            _packetHandlers.Add(remoteEndpoint, new PacketHandlerContainer { secret = secret, packetHandler = packethandler });
        }


        /// <summary>
        /// Start listening for requests
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                var endpoint = (IPEndPoint)_server.Client.LocalEndPoint;
                _log.Info($"Starting Radius server on {endpoint.Address}:{endpoint.Port}");

                Running = true;
                _server.BeginReceive(new AsyncCallback(ReceiveCallback), null);

                _log.Info("Server started");
            }
            else
            {
                _log.Warn("Server already started");
            }
        }


        /// <summary>
        /// Stop listening
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                _log.Info("Stopping server");

                Running = false;
                _server.Close();

                _log.Info("Stopped");
            }
            else
            {
                _log.Warn("Server already stopped");
            }
        }


        /// <summary>
        /// Receive packets
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (Running)
            {
                var sender = new IPEndPoint(IPAddress.Any, 0);
                var packetbytes = _server.EndReceive(ar, ref sender);

                // Immediately start listening for the next packet
                _server.BeginReceive(new AsyncCallback(ReceiveCallback), null);

                try
                {
                    _log.Debug($"Received packet from {sender.Address}:{sender.Port}");
                    if (_packetHandlers.TryGetValue(sender.Address, out var handler))
                    {
                        var responsePacket = GetResponsePacket(handler.packetHandler, handler.secret, packetbytes, sender);
                        SendResponsePacket(responsePacket, sender, _dictionary);
                    }
                    else
                    {
                        _log.Error($"No packet handler found for remote ip {sender.Address}");
                        var packet = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.ASCII.GetBytes("wut"));
                        DumpPacket(packet);
                    }

                }
                catch (ArgumentException ex)
                {
                    _log.Warn($"Ignoring malformed(?) packet received from {sender.Address}:{sender.Port}", ex);
                    DumpPacketBytes(packetbytes);
                }
                catch (OverflowException ex)
                {
                    _log.Warn($"Ignoring malformed(?) packet received from {sender.Address}:{sender.Port}", ex);
                    DumpPacketBytes(packetbytes);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _log.Error($"Failed to receive packet from {sender.Address}:{sender.Port}", ex);
                        DumpPacketBytes(packetbytes);
                    }
                    catch (Exception iex)
                    {
                        _log.Warn("Couldnt get sender?!", iex);
                        _log.Error("Failed to receive packet", ex);
                    }
                }
            }
        }


        /// <summary>
        /// Parses a packet and gets a response packet from the handler
        /// </summary>
        /// <param name="packethandler"></param>
        /// <param name="secret"></param>
        /// <param name="packetbytes"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public IRadiusPacket GetResponsePacket(IPacketHandler packethandler, String secret, Byte[] packetbytes, IPEndPoint sender)
        {
            var requestPacket = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.ASCII.GetBytes(secret));
            if (requestPacket.Code == PacketCode.AccountingRequest)
            {
                _log.Info($"Received {requestPacket.Code} {requestPacket.GetAttribute<AcctStatusType>("Acct-Status-Type")} from {sender.Address}:{sender.Port} Id={requestPacket.Identifier}");
            }
            else
            {
                _log.Info($"Received {requestPacket.Code} from {sender.Address}:{sender.Port} Id={requestPacket.Identifier}");
            }

            if (_log.IsDebugEnabled)
            {
                DumpPacket(requestPacket);
            }

            // Todo add message authenticator and accounting request authenticator validation

            // Handle status server requests in server outside packet handler
            if (requestPacket.Code == PacketCode.StatusServer)
            {
                return requestPacket.CreateResponsePacket(PacketCode.AccessAccept);
            }

            _log.Debug($"Handling packet for remote ip {sender.Address} with {packethandler.GetType()}");

            var sw = new Stopwatch();
            sw.Start();
            var responsePacket = packethandler.HandlePacket(requestPacket);
            sw.Stop();
            _log.Debug($"{sender.Address}:{sender.Port} Id={responsePacket.Identifier}, Received {responsePacket.Code} from handler in {sw.ElapsedMilliseconds}ms");

            if (requestPacket.Attributes.ContainsKey("Proxy-State"))
            {
                responsePacket.Attributes.Add("Proxy-State", requestPacket.Attributes.SingleOrDefault(o => o.Key == "Proxy-State").Value);
            }

            return responsePacket;
        }


        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="responsepacket"></param>
        /// <param name="sender"></param>
        private void SendResponsePacket(IRadiusPacket responsepacket, IPEndPoint recipient, RadiusDictionary dictionary)
        {
            var responseBytes = GetBytes(responsepacket, dictionary);
            _server.Send(responseBytes, responseBytes.Length, recipient);   // todo thread safety... although this implementation will be implicitly thread safeish...
            _log.Info($"{responsepacket.Code} sent to {recipient.Address}:{recipient.Port} Id={responsepacket.Identifier}");
        }


        /// <summary>
        /// Get the raw packet bytes
        /// </summary>
        /// <returns></returns>
        public static Byte[] GetBytes(IRadiusPacket packet, RadiusDictionary dictionary)
        {
            var packetbytes = new Byte[20]; // Should be 20 + AVPs...
            packetbytes[0] = (Byte)packet.Code;
            packetbytes[1] = packet.Identifier;

            foreach (var attribute in packet.Attributes)
            {
                // todo add logic to check attribute object type matches type in dictionary?
                foreach (var value in attribute.Value)
                {
                    var contentBytes = GetAttributeValueBytes(value);
                    var headerBytes = new Byte[0];

                    // Figure out what kind of attribute this is
                    var attributeType = dictionary.Attributes.SingleOrDefault(o => o.Value.Name == attribute.Key);
                    if (dictionary.Attributes.ContainsValue(attributeType.Value))
                    {
                        headerBytes = new Byte[2];
                        headerBytes[0] = attributeType.Value.Value;
                    }
                    else
                    {
                        // Maybe this is a vendor attribute?
                        var vendorAttributeType = dictionary.VendorAttributes.SingleOrDefault(o => o.Name == attribute.Key);
                        if (vendorAttributeType != null)
                        {
                            headerBytes = new Byte[8];
                            headerBytes[0] = 26; // VSA type

                            var vendorId = BitConverter.GetBytes(vendorAttributeType.VendorId);
                            Array.Reverse(vendorId);
                            Buffer.BlockCopy(vendorId, 0, headerBytes, 2, 4);
                            headerBytes[6] = (Byte)vendorAttributeType.Code;
                            headerBytes[7] = (Byte)(2 + contentBytes.Length);  // length of the vsa part
                        }
                        else
                        {
                            _log.Debug($"Ignoring unknown attribute {attribute.Key}");
                        }
                    }

                    var attributeBytes = new Byte[headerBytes.Length + contentBytes.Length];
                    Buffer.BlockCopy(headerBytes, 0, attributeBytes, 0, headerBytes.Length);
                    Buffer.BlockCopy(contentBytes, 0, attributeBytes, headerBytes.Length, contentBytes.Length);
                    attributeBytes[1] = (Byte)attributeBytes.Length;

                    // Add attribute to packet
                    Array.Resize(ref packetbytes, packetbytes.Length + attributeBytes.Length);
                    Buffer.BlockCopy(attributeBytes, 0, packetbytes, packetbytes.Length - attributeBytes.Length, attributeBytes.Length);
                }
            }

            // Note the order of the bytes...
            var responselengthbytes = BitConverter.GetBytes(packetbytes.Length);
            packetbytes[2] = responselengthbytes[1];
            packetbytes[3] = responselengthbytes[0];

            // Done last... includes total packet length
            var responseAuthenticator = CreateResponseAuthenticator(packet.SharedSecret, packet.Authenticator, packetbytes);
            Buffer.BlockCopy(responseAuthenticator, 0, packetbytes, 4, 16);

            return packetbytes;
        }


        /// <summary>
        /// Gets the byte representation of an attribute object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Byte[] GetAttributeValueBytes(Object value)
        {
            byte[] contentBytes;
            if (value.GetType() == typeof(String))
            {
                contentBytes = Encoding.Default.GetBytes((String)value);
            }
            else if (value.GetType() == typeof(UInt32))
            {
                contentBytes = BitConverter.GetBytes((UInt32)value);
                Array.Reverse(contentBytes);
            }
            else if (value.GetType() == typeof(Byte[]))
            {
                contentBytes = (Byte[])value;
            }
            else if (value.GetType() == typeof(IPAddress))
            {
                contentBytes = ((IPAddress)value).GetAddressBytes();
            }
            else
            {
                throw new NotImplementedException();
            }

            return contentBytes;
        }


        /// <summary>
        /// Creates a response authenticator
        /// Response authenticator = MD5(Code+ID+Length+RequestAuth+Attributes+Secret)
        /// Actually this means it is the response packet with the request authenticator and secret...
        /// </summary>
        /// <param name="radiusSharedSecret"></param>
        /// <param name="requestAuthenticator"></param>
        /// <param name="packetBytes"></param>
        /// <returns>Response authenticator for the packet</returns>
        private static Byte[] CreateResponseAuthenticator(Byte[] radiusSharedSecret, Byte[] requestAuthenticator, Byte[] packetBytes)
        {
            var responseAuthenticator = new Byte[packetBytes.Length + radiusSharedSecret.Length];
            Buffer.BlockCopy(packetBytes, 0, responseAuthenticator, 0, packetBytes.Length);
            Buffer.BlockCopy(requestAuthenticator, 0, responseAuthenticator, 4, 16);
            Buffer.BlockCopy(radiusSharedSecret, 0, responseAuthenticator, packetBytes.Length, radiusSharedSecret.Length);

            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(responseAuthenticator);
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


        /// <summary>
        /// Convert byte array to hex string representation.
        /// Used for debugging, testing and packet dumps
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String ByteArrayToString(Byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat($"{b:x2}");
            }
            return hex.ToString();
        }


        /// <summary>
        /// Log packet bytes for debugging
        /// </summary>
        /// <param name="packetbytes"></param>
        private static void DumpPacketBytes(Byte[] packetbytes)
        {
            try
            {
                _log.Debug(ByteArrayToString(packetbytes));
            }
            catch (Exception)
            {
                _log.Warn("duh");
            }
        }
    

        /// <summary>
        /// Dump the packet attributes to the log
        /// </summary>
        /// <param name="packet"></param>
        private static void DumpPacket(IRadiusPacket packet)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Packet dump for {packet.Identifier}:");
            foreach (var attribute in packet.Attributes)
            {
                if (attribute.Key == "User-Password")
                {
                    sb.AppendLine($"{attribute.Key} length : {attribute.Value.First().ToString().Length}");
                }
                else
                {
                    attribute.Value.ForEach(o => sb.AppendLine($"{attribute.Key} : {o} [{o.GetType()}]"));
                }
            }
            _log.Debug(sb.ToString());
        }
    }
}