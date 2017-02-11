using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RadiusServer));
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
                var sender = new IPEndPoint(IPAddress.Any, 0);
                var packetbytes = _server.EndReceive(ar, ref sender);

                // Immediately start listening for the next packet
                _server.BeginReceive(new AsyncCallback(ReceiveCallback), null);

                try
                {
                    _log.Debug($"Received packet from {sender.Address}:{sender.Port}");

                    if (!_packetHandlers.ContainsKey(sender.Address))
                    {
                        _log.Error($"No packet handler found for remote ip {sender.Address}");
                        return;
                    }

                    var handler = _packetHandlers[sender.Address];
                    _log.Debug($"Handling packet for remote ip {handler.packetHandler.GetType()} with {sender.Address}");

                    var responsePacket = GetResponsePacket(handler.packetHandler, handler.secret, packetbytes, sender);
                    SendResponsePacket(responsePacket, sender);
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
        /// Log packet bytes for debugging
        /// </summary>
        /// <param name="packetbytes"></param>
        private void DumpPacketBytes(byte[] packetbytes)
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
        /// Parses a packet and gets a response packet from the handler
        /// </summary>
        /// <param name="packethandler"></param>
        /// <param name="secret"></param>
        /// <param name="packetbytes"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public IRadiusPacket GetResponsePacket(IPacketHandler packethandler, String secret, Byte[] packetbytes, IPEndPoint sender)
        {
            var packet = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.ASCII.GetBytes(secret));
            if (packet.Code == PacketCode.AccountingRequest)
            {
                _log.Info($"Received {packet.Code} {packet.GetAttribute<AcctStatusTypes>("Acct-Status-Type")} from {sender.Address}:{sender.Port} Id={packet.Identifier}");
            }
            else
            {
                _log.Info($"Received {packet.Code} from {sender.Address}:{sender.Port} Id={packet.Identifier}");
            }

            if (_log.IsDebugEnabled)
            {
                DumpPacket(packet);
            }

            var sw = new Stopwatch();
            sw.Start();
            var responsepacket = packethandler.HandlePacket(packet);
            sw.Stop();
            _log.Debug($"{sender.Address}:{sender.Port} Id={responsepacket.Identifier}, Received {responsepacket.Code} from handler in {sw.ElapsedMilliseconds}ms");
            return responsepacket;
        }


        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="responsepacket"></param>
        /// <param name="sender"></param>
        private void SendResponsePacket(IRadiusPacket responsepacket, IPEndPoint recipient)
        {
            using (var client = new UdpClient())
            {
                var responseBytes = GetBytes(responsepacket);
                client.Send(responseBytes, responseBytes.Length, recipient);
            }

            _log.Info($"{responsepacket.Code} sent to {recipient.Address}:{recipient.Port} Id={responsepacket.Identifier}");
        }


        /// <summary>
        /// Dump the packet attributes to the log
        /// </summary>
        /// <param name="packet"></param>
        private void DumpPacket(IRadiusPacket packet)
        {
            foreach (var attribute in packet.Attributes)
            {
                if (attribute.Key == "User-Password")
                {
                    _log.Debug($"{attribute.Key} length : {attribute.Value.First().ToString().Length}");
                }
                else
                {
                    attribute.Value.ForEach(o => _log.Debug($"{attribute.Key} : {o} [{o.GetType()}]"));
                }
            }
        }


        /// <summary>
        /// Get the raw packet bytes
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes(IRadiusPacket packet)
        {
            var packetbytes = new Byte[20]; // Should be 20 + AVPs...
            packetbytes[0] = (Byte)packet.Code;
            packetbytes[1] = packet.Identifier;

            foreach (var attribute in packet.Attributes)
            {
                // todo add logic to check attribute type matches actual type in dictionary?
                foreach (var value in attribute.Value)
                {
                    var contentBytes = new Byte[0];
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


                    // Figure out what kind of attribute this is
                    var attributeType = _dictionary.Attributes.SingleOrDefault(o => o.Value.Name == attribute.Key);
                    if (_dictionary.Attributes.ContainsValue(attributeType.Value))
                    {
                        var attributeLength = (Byte)(2 + contentBytes.Length);
                        var attributeBytes = new Byte[attributeLength];
                        attributeBytes[0] = attributeType.Value.Value;
                        attributeBytes[1] = attributeLength;
                        Buffer.BlockCopy(contentBytes, 0, attributeBytes, 2, contentBytes.Length);

                        // Add to packet
                        Array.Resize(ref packetbytes, packetbytes.Length + attributeBytes.Length);
                        Buffer.BlockCopy(attributeBytes, 0, packetbytes, packetbytes.Length - attributeBytes.Length, attributeBytes.Length);
                    }
                    else
                    {
                        // Maybe this is a vendor attribute?
                        var vendorAttributeType = _dictionary.VendorAttributes.SingleOrDefault(o => o.Name == attribute.Key);
                        if (vendorAttributeType != null)
                        {
                            var attributeLength = (Byte)(8 + contentBytes.Length);  // minimum length for a VSA is 8 + content
                            var attributeBytes = new Byte[attributeLength];
                            attributeBytes[0] = 26; // VSA type
                            attributeBytes[1] = attributeLength;    // total length

                            var vendorId = BitConverter.GetBytes(vendorAttributeType.VendorId);
                            Array.Reverse(vendorId);
                            Buffer.BlockCopy(vendorId, 0, attributeBytes, 2, 4);
                            attributeBytes[6] = (Byte)vendorAttributeType.Code; // todo, should be byte, some use more? wtf?
                            attributeBytes[7] = (Byte)(2 + contentBytes.Length);  // length of the vsa part
                            Buffer.BlockCopy(contentBytes, 0, attributeBytes, 8, contentBytes.Length);

                            // Add to packet
                            Array.Resize(ref packetbytes, packetbytes.Length + attributeBytes.Length);
                            Buffer.BlockCopy(attributeBytes, 0, packetbytes, packetbytes.Length - attributeBytes.Length, attributeBytes.Length);
                        }
                        else
                        {
                            _log.Debug($"Ignoring unknown attribute {attribute.Key}");
                        }
                    }
                }
            }


            // Note the order of the bytes...
            var responselengthbytes = BitConverter.GetBytes(packetbytes.Length);
            packetbytes[2] = responselengthbytes[1];
            packetbytes[3] = responselengthbytes[0];

            // todo hack...
            var attributes = new Byte[packetbytes.Length - 20];
            Buffer.BlockCopy(packetbytes, 20, attributes, 0, attributes.Length);

            // Done last... includes packet length
            var responseAuthenticator = CreateResponseAuthenticator(packet.Code, packet.SharedSecret, packet.Identifier, packet.Authenticator, (Int16)packetbytes.Length, attributes);
            Buffer.BlockCopy(responseAuthenticator, 0, packetbytes, 4, 16);

            return packetbytes;
        }


        /// <summary>
        /// Creates a response authenticator
        /// Response authenticator = MD5(Code+ID+Length+RequestAuth+Attributes+Secret)
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="radiusSharedSecret"></param>
        /// <param name="identifier"></param>
        /// <param name="requestAuthenticator"></param>
        /// <param name="responselength"></param>
        /// <param name="attributes"></param>
        /// <returns>Response authenticator for the packet</returns>
        private Byte[] CreateResponseAuthenticator(PacketCode responseCode, Byte[] radiusSharedSecret, Byte identifier, Byte[] requestAuthenticator, Int16 responselength, Byte[] attributes)
        {
            var responseAuthenticator = new Byte[20 + radiusSharedSecret.Length + attributes.Length];
            responseAuthenticator[0] = (Byte)responseCode;
            responseAuthenticator[1] = identifier;

            var responselengthbytes = BitConverter.GetBytes(responselength);
            responseAuthenticator[2] = responselengthbytes[1];
            responseAuthenticator[3] = responselengthbytes[0];

            Buffer.BlockCopy(requestAuthenticator, 0, responseAuthenticator, 4, 16);
            Buffer.BlockCopy(attributes, 0, responseAuthenticator, 20, attributes.Length);
            Buffer.BlockCopy(radiusSharedSecret, 0, responseAuthenticator, 20 + attributes.Length, radiusSharedSecret.Length);

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


        public static String ByteArrayToString(Byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat($"{b:x2}");
            }
            return hex.ToString();
        }
    }
}