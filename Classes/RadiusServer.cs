using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RadiusServer));
        private IUdpClientWrapper _server;
        private readonly IPEndPoint _serverEndpoint;
        private readonly RadiusDictionary _dictionary;
        private readonly RadiusServerType _serverType;
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
        public RadiusServer(IPEndPoint serverEndpoint, RadiusDictionary dictionary, RadiusServerType serverType)
        {
            _serverEndpoint = serverEndpoint;
            _dictionary = dictionary;
            _serverType = serverType;
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
                _server = new UdpClientWrapper(_serverEndpoint);
                Running = true;
                _log.Info($"Starting Radius server on {_serverEndpoint.Address}:{_serverEndpoint.Port}");
                var receiveTask = StartReceiveLoop();
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
                _server.Dispose();
                _log.Info("Stopped");
            }
            else
            {
                _log.Warn("Server already stopped");
            }
        }


        /// <summary>
        /// Start the loop used for receiving packets
        /// </summary>
        /// <returns></returns>
        private async Task StartReceiveLoop()
        {
            while (Running)
            {
                try
                {
                    var response = await _server.ReceiveAsync();
                    _log.Debug($"Received packet from {response.RemoteEndPoint.Address}:{response.RemoteEndPoint.Port}");
                    Task.Run(() => { HandlePacket(response.RemoteEndPoint, response.Buffer); });
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    _log.Fatal("Something went wrong receiving packet", ex);
                }
            }
        }


        /// <summary>
        /// Used to handle the packets asynchronously
        /// </summary>
        /// <param name="ar"></param>
        private void HandlePacket(IPEndPoint sender, Byte[] packetbytes)
        {
            try
            {
                if (_packetHandlers.TryGetValue(sender.Address, out var handler))
                {
                    var responsePacket = GetResponsePacket(handler.packetHandler, handler.secret, packetbytes, sender);
                    if (responsePacket != null)
                    {
                        SendResponsePacket(responsePacket, sender, _dictionary);
                    }
                }
                else
                {
                    _log.Error($"No packet handler found for remote ip {sender.Address}");
                    var packet = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.UTF8.GetBytes("wut"));
                    DumpPacket(packet);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OverflowException)
            {
                _log.Warn($"Ignoring malformed(?) packet received from {sender.Address}:{sender.Port}", ex);
                DumpPacketBytes(packetbytes);
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to receive packet from {sender.Address}:{sender.Port}", ex);
                DumpPacketBytes(packetbytes);
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
            var requestPacket = RadiusPacket.ParseRawPacket(packetbytes, _dictionary, Encoding.UTF8.GetBytes(secret));
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

            if (requestPacket.Attributes.ContainsKey("Message-Authenticator"))
            {
                var messageAuthenticator = Utils.ByteArrayToString(requestPacket.GetAttribute<Byte[]>("Message-Authenticator"));
                var calculatedMessageAuthenticator = RadiusPacket.CalculateMessageAuthenticator(requestPacket, _dictionary);
                if (messageAuthenticator != calculatedMessageAuthenticator)
                {
                    _log.Warn($"Invalid Message-Authenticator in packet {requestPacket.Identifier} from {sender.Address}:{sender.Port}, check secret");
                    return null;
                }
            }

            // Handle status server requests in server outside packet handler
            if (requestPacket.Code == PacketCode.StatusServer)
            {
                if (_serverType == RadiusServerType.Authentication)
                {
                    _log.Debug($"Sending AccessAccept for StatusServer request from {sender.Address}:{sender.Port}");
                    return requestPacket.CreateResponsePacket(PacketCode.AccessAccept);
                }
                else if (_serverType == RadiusServerType.Accounting)
                {
                    _log.Debug($"Sending AccountingResponse for StatusServer request from {sender.Address}:{sender.Port}");
                    return requestPacket.CreateResponsePacket(PacketCode.AccountingResponse);
                }
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
        /// Get the raw packet bytes with calculated response authenticator
        /// </summary>
        /// <returns></returns>
        public static Byte[] GetBytes(IRadiusPacket packet, RadiusDictionary dictionary)
        {
            var packetBytes = packet.GetBytes(dictionary);
            var responseAuthenticator = CreateResponseAuthenticator(packet.SharedSecret, packet.Authenticator, packetBytes);
            Buffer.BlockCopy(responseAuthenticator, 0, packetBytes, 4, 16);
            return packetBytes;
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
            _server?.Dispose();
        }


        /// <summary>
        /// Log packet bytes for debugging
        /// </summary>
        /// <param name="packetbytes"></param>
        private static void DumpPacketBytes(Byte[] packetbytes)
        {
            try
            {
                _log.Debug(Utils.ByteArrayToString(packetbytes));
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