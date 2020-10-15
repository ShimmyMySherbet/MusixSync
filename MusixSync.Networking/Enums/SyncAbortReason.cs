using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Networking.Enums
{
    public enum SyncAbortReason
    {
        Error = 0,
        NoFilesOnServer = 1,
        NoNeededFiles = 2,
        SyncDenied = 3,
        PartnerAbortedSync = 4,
        SyncWasCanceled = 5
    }
}
