using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public enum AcctStatusTypes : uint
    {
        Start = 1,
        Stop = 2,
        InterimUpdate = 3,
        AccountingOn = 7,
        AccountingOff = 8
    }
}
