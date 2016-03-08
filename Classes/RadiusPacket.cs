using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Flexinets.Radius
{
    /// <summary>
    /// This class encapsulates a Radius packet and presents it in a more readable form
    /// </summary>
    public class RadiusPacket : IRadiusPacket
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RadiusPacket));

        public PacketCode Code
        {
            get;
            private set;
        }
        public Byte Identifier
        {
            get;
            private set;
        }
        public Int16 Length
        {
            get
            {
                // todo wtf?
                // Code + Identifier + Length + Authenticator... + attributes of course...
                return 1 + 1 + 2 + 16;
            }
        }
        public Byte[] Authenticator
        {
            get;
            private set;
        }
        public IDictionary<String, Object> Attributes
        {
            get;
            set;
        }
        public Byte[] SharedSecret
        {
            get;
            private set;
        }


        /// <summary>
        /// Parses the udp raw packet and returns a more easily readable IRadiusPacket
        /// </summary>
        /// <param name="packetBytes"></param>
        public static IRadiusPacket ParseRawPacket(Byte[] packetBytes, RadiusDictionary dictionary, Byte[] radiusSharedSecret)
        {
            // Check the packet length and make sure its valid
            Array.Reverse(packetBytes, 2, 2);
            if (packetBytes.Length != BitConverter.ToUInt16(packetBytes, 2))
            {
                throw new Exception("Packet length does not match");
            }

            var radiusPacket = new RadiusPacket
            {
                SharedSecret = radiusSharedSecret,
                Attributes = new Dictionary<String, object>(),
                Identifier = packetBytes[1],
                Code = (PacketCode)packetBytes[0],
                Authenticator = new Byte[16]
            };

            Buffer.BlockCopy(packetBytes, 4, radiusPacket.Authenticator, 0, 16);


            // The rest are attribute value pairs
            Int16 i = 20;
            while (i < packetBytes.Length)
            {
                var typecode = packetBytes[i];
                var length = packetBytes[i + 1];
                var contentBytes = new Byte[length - 2];
                Buffer.BlockCopy(packetBytes, i + 2, contentBytes, 0, length - 2);

                if (typecode == 26)
                {
                    var vsa = new VendorSpecificAttribute(contentBytes);
                    var attributeDefinition = dictionary.VendorAttributes.FirstOrDefault(o => o.VendorId == vsa.VendorId && o.Code == vsa.VendorCode);
                    if (attributeDefinition == null)
                    {
                        _log.WarnFormat("Unknown vsa: {0}:{1}", vsa.VendorId, vsa.VendorCode);
                    }
                    else
                    {
                        var content = ParseContentBytes(vsa.Value, attributeDefinition.Type, typecode, radiusPacket.Authenticator, radiusPacket.SharedSecret);
                        radiusPacket.Attributes.Add(attributeDefinition.Name, content);
                    }
                }
                else
                {
                    var attributeDefinition = dictionary.Attributes[typecode];
                    var content = ParseContentBytes(contentBytes, attributeDefinition.Type, typecode, radiusPacket.Authenticator, radiusPacket.SharedSecret);
                    radiusPacket.Attributes.Add(attributeDefinition.Name, content);
                }

                i += length;
            }

            return radiusPacket;
        }


        /// <summary>
        /// Parses the content and returns an object of proper type
        /// </summary>
        /// <param name="contentBytes"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="authenticator"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private static Object ParseContentBytes(Byte[] contentBytes, String type, UInt32 code, Byte[] authenticator, Byte[] secret)
        {
            switch (type)
            {
                case "string":
                    return Encoding.Default.GetString(contentBytes);

                case "binary":
                    // If this is a password attribute it must be decrypted
                    if (code == 2)
                    {
                        return RadiusPassword.Decrypt(secret, authenticator, contentBytes);
                    }
                    // Otherwise just dump the bytes into the attribute
                    else
                    {
                        return contentBytes;
                    }

                case "integer":
                    return BitConverter.ToUInt32(contentBytes.Reverse().ToArray(), 0);

                case "ipaddr":
                    return new IPAddress(contentBytes);

                default:
                    return null;
            }
        }


        /// <summary>
        /// Creates a response packet with authenticator, identifier, secret etc set
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        public IRadiusPacket CreateResponsePacket(PacketCode responseCode)
        {
            return new RadiusPacket
            {
                Code = responseCode,
                SharedSecret = SharedSecret,
                Identifier = Identifier,
                Authenticator = Authenticator,
            };
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

            using (MD5 x = MD5CryptoServiceProvider.Create())
            {
                return x.ComputeHash(responseAuthenticator);
            }
        }


        /// <summary>
        /// Get the raw packet bytes
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes()
        {
            var packetbytes = new Byte[Length]; // Should be 20 + AVPs...
            packetbytes[0] = (Byte)Code;
            packetbytes[1] = Identifier;

            // Note the order of the bytes...
            var responselengthbytes = BitConverter.GetBytes(Length);
            packetbytes[2] = responselengthbytes[1];
            packetbytes[3] = responselengthbytes[0];

            var responseAuthenticator = CreateResponseAuthenticator(Code, SharedSecret, Identifier, Authenticator, Length);
            Buffer.BlockCopy(responseAuthenticator, 0, packetbytes, 4, 16);

            return packetbytes;
        }


        /// <summary>
        /// Gets an attribute cast to type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetAttribute<T>(String name)
        {
            if (Attributes.ContainsKey(name))
            {
                return (T)Attributes[name];
            }

            return default(T);
        }
    }
}
