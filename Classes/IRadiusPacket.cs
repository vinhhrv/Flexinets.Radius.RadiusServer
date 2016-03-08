using System;
using System.Collections.Generic;

namespace Flexinets.Radius
{
    public interface IRadiusPacket
    {
        Int16 Length
        {
            get;
        }
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
        IDictionary<String, Object> Attributes
        {
            get;
        }
        IRadiusPacket CreateResponsePacket(PacketCode responseCode);

        T GetAttribute<T>(String name);
        Byte[] GetBytes();
    }
}
