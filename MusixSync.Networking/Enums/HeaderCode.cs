using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Networking.Enums
{
    public enum HeaderCode : byte
    {
        RequestFile = 0x11,
        TranserComplete = 0x12,
        SyncComplete = 0x13
    }
}
