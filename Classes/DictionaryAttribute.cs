using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    /// <summary>
    /// ATTRIBUTE	Framed-Protocol			7	integer
    /// </summary>
    public class DictionaryAttribute
    {
        public readonly Byte Value;
        public readonly String Name;
        public readonly String Type;

        public DictionaryAttribute(Byte value, String name, String type)
        {
            Value = value;
            Name = name;
            Type = type;
        }
    }
}
