using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    /// <summary>
    /// VENDORATTR      9 	Cisco-Maximun-Channels          235     integer
    /// </summary>
    public class DictionaryVendorAttribute
    {
        public readonly UInt32 VendorId;
        public readonly String Name;
        public readonly UInt32 Code;
        public readonly String Type;

        public DictionaryVendorAttribute(UInt32 vendorId, UInt32 code, String name, String type)
        {
            VendorId = vendorId;
            Name = name;
            Code = code;
            Type = type;
        }
    }
}
