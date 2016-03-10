﻿using System;
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

        T GetAttribute<T>(String name);

        void AddAttribute(String name, String value);
        void AddAttribute(String name, UInt32 value);
        void AddAttribute(String name, IPAddress value);
        void AddAttribute(String name, Byte[] value);
    }
}
