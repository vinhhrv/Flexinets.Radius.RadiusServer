using Flexinets.Radius.DictionaryAttributes;
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
        public Byte[] Authenticator
        {
            get;
            internal set;
        }
        public Byte[] SharedSecret
        {
            get;
            private set;
        }
        public Dictionary<Type, List<IAttribute>> TypedAttributes { get; private set; } = new Dictionary<Type, List<IAttribute>>();


        private RadiusPacket()
        {
        }


        /// <summary>
        /// Create a new packet
        /// </summary>
        /// <param name="code"></param>
        /// <param name="identifier"></param>
        /// <param name="secret"></param>
        public RadiusPacket(PacketCode code, Byte identifier, String secret)
        {
            Code = code;
            Identifier = identifier;
            SharedSecret = Encoding.UTF8.GetBytes(secret);
            Authenticator = new Byte[16];
            using (var csp = RandomNumberGenerator.Create())
            {
                csp.GetNonZeroBytes(Authenticator);
            }
        }


        /// <summary>
        /// Parses the udp raw packet and returns a more easily readable IRadiusPacket
        /// </summary>
        /// <param name="packetBytes"></param>
        /// <param name="dictionary"></param>
        /// <param name="sharedSecret"></param>
        public static IRadiusPacket ParseRawPacket(Byte[] packetBytes, RadiusDictionary dictionary, Byte[] sharedSecret)
        {
            // Check the packet length and make sure its valid
            Array.Reverse(packetBytes, 2, 2);
            var packetLength = BitConverter.ToUInt16(packetBytes, 2);
            if (packetBytes.Length != packetLength)
            {
                var message = $"Packet length does not match, expected: {packetLength}, actual: {packetBytes.Length}";
                _log.ErrorFormat(message);
                throw new InvalidOperationException(message);
            }

            var radiusPacket = new RadiusPacket
            {
                SharedSecret = sharedSecret,
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

                if (i + length > packetLength)
                {
                    throw new ArgumentOutOfRangeException("Go home roamserver, youre drunk");
                }
                var contentBytes = new Byte[length - 2];
                Buffer.BlockCopy(packetBytes, i + 2, contentBytes, 0, length - 2);

                try
                {
                    if (typecode == 26)
                    {
                        //var vsa = new VendorSpecificAttribute(contentBytes);
                        //var attributeDefinition = dictionary.VendorSpecificAttributes.FirstOrDefault(o => o.VendorId == vsa.VendorId && o.Code == vsa.VendorCode);
                        //if (attributeDefinition == null)
                        //{
                        //    _log.Debug($"Unknown vsa: {vsa.VendorId}:{vsa.VendorCode}");
                        //}
                        //else
                        //{
                        //    try
                        //    {
                        //        var content = ParseContentBytes(vsa.Value, attributeDefinition.Type, typecode, radiusPacket.Authenticator, radiusPacket.SharedSecret);
                        //        if (!radiusPacket.Attributes.ContainsKey(attributeDefinition.Name))
                        //        {
                        //            radiusPacket.Attributes.Add(attributeDefinition.Name, new List<object>());
                        //        }
                        //        radiusPacket.Attributes[attributeDefinition.Name].Add(content);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        _log.Error($"Something went wrong with vsa {attributeDefinition.Name}", ex);
                        //    }
                        //}
                    }
                    else
                    {
                        var attributeDefinition = dictionary.Attributes[typecode];
                        try
                        {
                            var content = ParseContentBytes(contentBytes, attributeDefinition.Type, typecode, radiusPacket.Authenticator, radiusPacket.SharedSecret);
                            if (!radiusPacket.Attributes.ContainsKey(attributeDefinition.Name))
                            {
                                radiusPacket.Attributes.Add(attributeDefinition.Name, new List<object>());
                            }
                            radiusPacket.Attributes[attributeDefinition.Name].Add(content);
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Something went wrong with vsa {attributeDefinition.Name}", ex);
                        }
                    }
                }
                catch (KeyNotFoundException)
                {
                    _log.Warn($"Attribute {typecode} not found in dictionary");
                }
                catch (Exception ex)
                {
                    _log.Error($"Something went wrong parsing attribute {typecode}", ex);
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
        /// <param name="sharedSecret"></param>
        /// <returns></returns>
        private static Object ParseContentBytes(Byte[] contentBytes, String type, UInt32 code, Byte[] authenticator, Byte[] sharedSecret)
        {
            switch (type)
            {
                case "string":
                    return Encoding.UTF8.GetString(contentBytes);

                case "tagged-string":
                    return Encoding.UTF8.GetString(contentBytes);

                case "octet":
                    // If this is a password attribute it must be decrypted
                    if (code == 2)
                    {
                        return RadiusPassword.Decrypt(sharedSecret, authenticator, contentBytes);
                    }
                    // Otherwise just dump the bytes into the attribute
                    else
                    {
                        return contentBytes;
                    }

                case "integer":
                    return BitConverter.ToUInt32(contentBytes.Reverse().ToArray(), 0);

                case "tagged-integer":
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
        /// Validates a message authenticator if one exists in the packet
        /// Message-Authenticator = HMAC-MD5 (Type, Identifier, Length, Request Authenticator, Attributes)
        /// The HMAC-MD5 function takes in two arguments:
        /// The payload of the packet, which includes the 16 byte Message-Authenticator field filled with zeros
        /// The shared secret
        /// https://www.ietf.org/rfc/rfc2869.txt
        /// </summary>
        /// <returns></returns>
        public static String CalculateMessageAuthenticator(IRadiusPacket packet, RadiusDictionary dictionary)
        {
            var checkPacket = ParseRawPacket(packet.GetBytes(dictionary), dictionary, packet.SharedSecret);    // This is a bit daft, but we dont want side effects do we...
            //checkPacket.GetTypedAttribute<MessageAuthenticatorAttribute>().Value = new Byte[16];
            // todo fix
            var bytes = checkPacket.GetBytes(dictionary);

            using (var md5 = new HMACMD5(checkPacket.SharedSecret))
            {
                return Utils.ByteArrayToString(md5.ComputeHash(bytes));
            }
        }


        /// <summary>
        /// Get the raw packet bytes
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes(RadiusDictionary dictionary)
        {
            var packetbytes = new Byte[20]; // Should be 20 + AVPs...
            packetbytes[0] = (Byte)Code;
            packetbytes[1] = Identifier;

            foreach (var attributes in TypedAttributes)
            {
                foreach (var attribute in attributes.Value)
                {
                    var contentBytes = attribute.GetBytes();
                    var headerBytes = new Byte[2];
                    headerBytes[0] = (Byte)attribute.Code;

                    if (attribute.Code == 2)
                    {
                        contentBytes = RadiusPassword.Encrypt(SharedSecret, Authenticator, contentBytes);
                    }


                    //else
                    //{
                    //    // Maybe this is a vendor attribute?
                    //    var vendorAttributeType = dictionary.VendorSpecificAttributes.SingleOrDefault(o => o.Name == attribute.Key);
                    //    if (vendorAttributeType != null)
                    //    {
                    //        headerBytes = new Byte[8];
                    //        headerBytes[0] = 26; // VSA type

                    //        var vendorId = BitConverter.GetBytes(vendorAttributeType.VendorId);
                    //        Array.Reverse(vendorId);
                    //        Buffer.BlockCopy(vendorId, 0, headerBytes, 2, 4);
                    //        headerBytes[6] = (Byte)vendorAttributeType.Code;
                    //        headerBytes[7] = (Byte)(2 + contentBytes.Length);  // length of the vsa part
                    //    }
                    //    else
                    //    {
                    //        _log.Debug($"Ignoring unknown attribute {attribute.Key}");
                    //    }
                    //}

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

            Buffer.BlockCopy(Authenticator, 0, packetbytes, 4, 16);

            return packetbytes;
        }




        public T GetTypedAttribute<T>() where T : IAttribute
        {
            if (TypedAttributes.ContainsKey(typeof(T)))
            {
                return (T)TypedAttributes[typeof(T)].First();
            }
            return default(T);
        }

        public List<T> GetTypedAttributes<T>() where T : IAttribute
        {
            if (TypedAttributes.ContainsKey(typeof(T)))
            {
                return TypedAttributes[typeof(T)].Cast<T>().ToList();
            }
            return default(List<T>);
        }

        public void AddTypedAttribute(IAttribute attribute)
        {
            if (!TypedAttributes.ContainsKey(attribute.GetType()))
            {
                TypedAttributes.Add(attribute.GetType(), new List<IAttribute>());
            }
            TypedAttributes[attribute.GetType()].Add(attribute);
        }
    }
}