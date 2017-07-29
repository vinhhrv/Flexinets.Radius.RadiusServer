using System;
using System.Net;

namespace Flexinets.Radius.DictionaryAttributes
{
    public class IPAddressAttribute : IAttribute
    {
        public IPAddress Value { get; protected set; }

        public virtual uint Code { get; protected set; }

        public Byte[] GetBytes()
        {
            return Value.GetAddressBytes();
        }
    }
}
