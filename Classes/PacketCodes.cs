using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public enum PacketCode
    {
        AccessRequest = 1,
        AccessAccept = 2,
        AccessReject = 3,
        AccountingRequest = 4,
        AccountingResponse = 5,
        AccessChallenge = 11,
        StatusServer = 12,
        StatusClient = 13,
        DisconnectRequest = 40
    }
}
