using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius.DictionaryAttributes
{
    public interface IAttribute
    {
        UInt32 Code { get; }
        Byte[] GetBytes();
    }
}
