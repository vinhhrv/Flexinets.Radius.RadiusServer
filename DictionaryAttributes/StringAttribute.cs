using System;
using System.Text;

namespace Flexinets.Radius.DictionaryAttributes
{
    public class StringAttribute : IAttribute
    {
        public String Value { get; protected set; }

        public virtual uint Code { get; protected set; }

        public Byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(Value);
        }
    }
}
