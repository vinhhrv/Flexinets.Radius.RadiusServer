using System;

namespace Flexinets.Radius.DictionaryAttributes
{
    public class OctetAttribute : IAttribute
    {
        public Byte[] Value { get; protected set; }

        public virtual uint Code { get; protected set; }

        public Byte[] GetBytes()
        {
            return Value;
        }
    }
}
