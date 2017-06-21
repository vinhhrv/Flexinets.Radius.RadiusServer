using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Flexinets.Radius;
using System.Net;
using System.IO;
using System.Reflection;

namespace RadiusServerTests
{
    [TestClass]
    public class RadiusServerTests
    {
        /// <summary>
        /// Send test packet and verify response packet
        /// Example from https://tools.ietf.org/html/rfc2865
        /// </summary>
        [TestMethod]
        public void TestResponsePacket()
        {
            var request = "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003";
            var expected = "0200002686fe220e7624ba2a1005f6bf9b55e0b20606000000010f06000000000e06c0a80103";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary, RadiusServerType.Authentication);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, Utils.StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = RadiusServer.GetBytes(response, dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(responseBytes));
        }


        /// <summary>
        /// Test status-server response
        /// Example from https://tools.ietf.org/html/rfc5997#section-6
        /// </summary>
        [TestMethod]
        public void TestStatusServerAuthenticationResponsePacket()
        {
            var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
            var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary, RadiusServerType.Authentication);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, Utils.StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = RadiusServer.GetBytes(response, dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(responseBytes));
        }


        /// <summary>
        /// Test status-server response
        /// Example from https://tools.ietf.org/html/rfc5997#section-6
        /// </summary>
        [TestMethod]
        public void TestStatusServerAccountingResponsePacket()
        {
            var request = "0cb30026925f6b66dd5fed571fcb1db7ad3882605012e8d6eabda910875cd91fdade26367858";
            var expected = "05b300140f6f92145f107e2f504e860a4860669c";  // Note the error in the RFC. First byte should be 05 not 02
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary, RadiusServerType.Accounting);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, Utils.StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = RadiusServer.GetBytes(response, dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(responseBytes));
        }


        /// <summary>
        /// Send test packet and verify response packet with proxy-state (21053135342105323330)
        /// Example from https://tools.ietf.org/html/rfc2865
        /// Modified to include two proxy states at the end
        /// </summary>
        [TestMethod]
        public void TestResponsePacketWithProxyState()
        {
            var request = "010000420f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a8011005060000000321053135342105323330";
            var expected = "02000030acf049cee1a3ed134316e5b3348cdf3c0606000000010f06000000000e06c0a8010321053135342105323330";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary, RadiusServerType.Authentication);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, Utils.StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = RadiusServer.GetBytes(response, dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(responseBytes));
        }


        /// <summary>
        /// Send test packet and verify response packet with proxy-state (21053135342105323330)
        /// Example from https://tools.ietf.org/html/rfc2865
        /// Modified to include two proxy states in the middle
        /// </summary>
        [TestMethod]
        public void TestResponsePacketWithProxyStateMiddle()
        {
            var request = "010000420f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a8011021053135342105323330050600000003";
            var expected = "02000030acf049cee1a3ed134316e5b3348cdf3c0606000000010f06000000000e06c0a8010321053135342105323330";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary, RadiusServerType.Authentication);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, Utils.StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = RadiusServer.GetBytes(response, dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(responseBytes));
        }


        /// <summary>
        /// Test parsing and rebuilding a packet
        /// </summary>
        [TestMethod]
        public void TestPacketParserAndAssembler()
        {
            var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
            var expected = request;
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var requestPacket = RadiusPacket.ParseRawPacket(Utils.StringToByteArray(request), dictionary, Encoding.UTF8.GetBytes(secret));
            var bytes = requestPacket.GetBytes(dictionary);

            Assert.AreEqual(expected, Utils.ByteArrayToString(bytes));
        }


        /// <summary>
        /// Test message authenticator validation success
        /// </summary>
        [TestMethod]
        public void TestMessageAuthenticatorValidationSuccess()
        {
            var expected = "5a665e2e1e8411f3e243822097c84fa3";
            var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var requestPacket = RadiusPacket.ParseRawPacket(Utils.StringToByteArray(request), dictionary, Encoding.UTF8.GetBytes(secret));
            var calculatedMessageAuthenticator = RadiusPacket.CalculateMessageAuthenticator(requestPacket, dictionary);
            Assert.AreEqual(expected, calculatedMessageAuthenticator);
        }


        /// <summary>
        /// Test message authenticator validation fail
        /// </summary>
        [TestMethod]
        public void TestMessageAuthenticatorValidationFail()
        {
            var expected = "5a665e2e1e8411f3e243822097c84fa3";
            var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
            var secret = "xyzzy5461durr";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var requestPacket = RadiusPacket.ParseRawPacket(Utils.StringToByteArray(request), dictionary, Encoding.UTF8.GetBytes(secret));
            var calculatedMessageAuthenticator = RadiusPacket.CalculateMessageAuthenticator(requestPacket, dictionary);
            Assert.AreNotEqual(expected, calculatedMessageAuthenticator);
        }
    }
}