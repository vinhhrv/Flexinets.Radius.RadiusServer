using NUnit.Framework;
using System.IO;
using System.Net;
using System.Reflection;

namespace Flexinets.Radius.Tests
{
    public class RadiusClientTests
    {
        /// <summary>
        /// Create packet and verify bytes
        /// Example from https://tools.ietf.org/html/rfc2865
        /// </summary>
        [TestCase]
        public void TestCreateAccessRequestPacket()
        {
            var expected = "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary";
            var dictionary = new RadiusDictionary(path);

            var packet = new RadiusPacket(PacketCode.AccessRequest, 0, secret, Utils.StringToByteArray("0f403f9473978057bd83d5cb98f4227a"));
            packet.AddAttribute("User-Name", "nemo");
            packet.AddAttribute("User-Password", "arctangent");
            packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
            packet.AddAttribute("NAS-Port", 3);

            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes(dictionary)));
        }
    }
}