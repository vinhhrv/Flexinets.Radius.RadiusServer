using System;

namespace Flexinets.Radius.DictionaryAttributes
{
    public class IntegerAttribute : IAttribute
    {
        public UInt32 Value { get; protected set; }

        public virtual uint Code { get; protected set; }

        public Byte[] GetBytes()
        {
            var bytes = BitConverter.GetBytes(Value);
            Array.Reverse(bytes);
            return bytes;
        }
    }
}
