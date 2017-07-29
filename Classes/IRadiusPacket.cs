using Flexinets.Radius.DictionaryAttributes;
using System;
using System.Collections.Generic;
using System.Net;

namespace Flexinets.Radius
{
    public interface IRadiusPacket
    {
        Byte Identifier
        {
            get;
        }
        Byte[] Authenticator
        {
            get;
        }
        Byte[] SharedSecret
        {
            get;
        }
        PacketCode Code
        {
            get;
        }
        IRadiusPacket CreateResponsePacket(PacketCode responseCode);


        Dictionary<Type, List<IAttribute>> TypedAttributes { get; };

        T GetTypedAttribute<T>() where T : IAttribute;
        List<T> GetTypedAttributes<T>() where T : IAttribute;

        void AddTypedAttribute(IAttribute attribute);


        Byte[] GetBytes(RadiusDictionary dictionary);
    }
}
