using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public class DictionaryAttribute
    {
        public readonly Byte Code;
        public readonly String Name;
        public readonly String Type;

        /// <summary>
        /// Create a dictionary rfc attribute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        public DictionaryAttribute(String name, Byte code, String type)
        {
            Code = code;
            Name = name;
            Type = type;
        }
    }
}
