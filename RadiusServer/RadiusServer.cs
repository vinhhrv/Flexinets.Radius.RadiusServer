using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RadiusServer));
        private IUdpClientWrapper _server;
        private readonly IPEndPoint _localEndpoint;
        private readonly RadiusDictionary _dictionary;
        private readonly RadiusServerType _serverType;
        private Int32 _concurrentHandlerCount = 0;
        private readonly Dictionary<IPAddress, (IPacketHandler packetHandler, String secret)> _packetHandlers = new Dictionary<IPAddress, (IPacketHandler, String)>();

        public Boolean Running
        {
            get;
            private set;
        }


        /// <summary>
        /// Create a new server on endpoint
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <param name="dictionary"></param>
        /// <param name="serverType"></param>
        public RadiusServer(IPEndPoint localEndpoint, RadiusDictionary dictionary, RadiusServerType serverType)
        {
            _localEndpoint = localEndpoint;
            _dictionary = dictionary;
            _serverType = serverType;
        }


        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteEndpoint"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetHandler"></param>
        public void AddPacketHandler(IPAddress remoteEndpoint, String sharedSecret, IPacketHandler packetHandler)
        {
            _log.Info($"Adding packet handler of type {packetHandler.GetType()} for remote IP {remoteEndpoint.ToString()} to {_serverType}Server");
            _packetHandlers.Add(remoteEndpoint, (packetHandler, sharedSecret));
        }


        /// <summary>
        /// Start listening for requests
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                _server = new UdpClientWrapper(_localEndpoint);
                Running = true;
                _log.Info($"Starting Radius server on {_localEndpoint}");
                var receiveTask = StartReceiveLoopAsync();
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
                _server?.Dispose();
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
        private async Task StartReceiveLoopAsync()
        {
            while (Running)
            {
                try
                {
                    var response = await _server.ReceiveAsync();
                    Task.Factory.StartNew(() => HandlePacket(response.RemoteEndPoint, response.Buffer), TaskCreationOptions.LongRunning);
                }
                catch (ObjectDisposedException) { } // This is thrown when udpclient is disposed, can be safely ignored
                catch (Exception ex)
                {
                    _log.Fatal("Something went wrong receiving packet", ex);
                }
            }
        }


        /// <summary>
        /// Used to handle the packets asynchronously
        /// </summary>
        /// <param name="remoteEndpoint"></param>
        /// <param name="packetBytes"></param>
        private void HandlePacket(IPEndPoint remoteEndpoint, Byte[] packetBytes)
        {
            try
            {
                _log.Debug($"Received packet from {remoteEndpoint}, Concurrent handlers count: {Interlocked.Increment(ref _concurrentHandlerCount)}");

                if (_packetHandlers.TryGetValue(remoteEndpoint.Address, out var handler))
                {
                    var responsePacket = GetResponsePacket(handler.packetHandler, handler.secret, packetBytes, remoteEndpoint);
                    if (responsePacket != null)
                    {
                        SendResponsePacket(responsePacket, remoteEndpoint, _dictionary);
                    }
                }
                else
                {
                    _log.Error($"No packet handler found for remote ip {remoteEndpoint}");
                    var packet = RadiusPacket.Parse(packetBytes, _dictionary, Encoding.UTF8.GetBytes("wut"));
                    DumpPacket(packet);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OverflowException)
            {
                _log.Warn($"Ignoring malformed(?) packet received from {remoteEndpoint}", ex);
                DumpPacketBytes(packetBytes);
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to receive packet from {remoteEndpoint}", ex);
                DumpPacketBytes(packetBytes);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentHandlerCount);
            }
        }


        /// <summary>
        /// Parses a packet and gets a response packet from the handler
        /// </summary>
        /// <param name="packetHandler"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetBytes"></param>
        /// <param name="remoteEndpoint"></param>
        /// <returns></returns>
        public IRadiusPacket GetResponsePacket(IPacketHandler packetHandler, String sharedSecret, Byte[] packetBytes, IPEndPoint remoteEndpoint)
        {
            var requestPacket = RadiusPacket.Parse(packetBytes, _dictionary, Encoding.UTF8.GetBytes(sharedSecret));
            _log.Info($"Received {requestPacket.Code} from {remoteEndpoint} Id={requestPacket.Identifier}");

            if (_log.IsDebugEnabled)
            {
                DumpPacket(requestPacket);
                DumpPacketBytes(packetBytes);
            }

            if (requestPacket.Attributes.ContainsKey("Message-Authenticator"))
            {
                var messageAuthenticator = requestPacket.GetAttribute<Byte[]>("Message-Authenticator");
                var calculatedMessageAuthenticator = RadiusPacket.CalculateMessageAuthenticator(requestPacket, _dictionary);
                if (!messageAuthenticator.SequenceEqual(calculatedMessageAuthenticator))
                {
                    _log.Warn($"Invalid Message-Authenticator in packet {requestPacket.Identifier} from {remoteEndpoint}, check secret");
                    return null;
                }
            }

            // Handle status server requests in server outside packet handler
            if (requestPacket.Code == PacketCode.StatusServer)
            {
                var responseCode = _serverType == RadiusServerType.Authentication ? PacketCode.AccessAccept : PacketCode.AccountingResponse;
                _log.Debug($"Sending {responseCode} for StatusServer request from {remoteEndpoint}");
                return requestPacket.CreateResponsePacket(responseCode);
            }

            _log.Debug($"Handling packet for remote ip {remoteEndpoint.Address} with {packetHandler.GetType()}");

            var sw = Stopwatch.StartNew();
            var responsePacket = packetHandler.HandlePacket(requestPacket);
            sw.Stop();
            _log.Debug($"{remoteEndpoint} Id={responsePacket.Identifier}, Received {responsePacket.Code} from handler in {sw.ElapsedMilliseconds}ms");
            if (sw.ElapsedMilliseconds >= 5000)
            {
                _log.Warn($"Slow response for Id {responsePacket.Identifier}, check logs");
            }

            if (requestPacket.Attributes.ContainsKey("Proxy-State"))
            {
                responsePacket.Attributes.Add("Proxy-State", requestPacket.Attributes.SingleOrDefault(o => o.Key == "Proxy-State").Value);
            }

            //responsePacket.au

            return responsePacket;
        }


        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="responsePacket"></param>
        /// <param name="remoteEndpoint"></param>
        /// <param name="dictionary"></param>
        private void SendResponsePacket(IRadiusPacket responsePacket, IPEndPoint remoteEndpoint, RadiusDictionary dictionary)
        {
            var responseBytes = responsePacket.GetBytes(dictionary);
            _server.Send(responseBytes, responseBytes.Length, remoteEndpoint);   // todo thread safety... although this implementation will be implicitly thread safeish...
            _log.Info($"{responsePacket.Code} sent to {remoteEndpoint} Id={responsePacket.Identifier}");
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
        /// <param name="packetBytes"></param>
        private static void DumpPacketBytes(Byte[] packetBytes)
        {
            try
            {
                _log.Debug(Utils.ByteArrayToString(packetBytes));
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