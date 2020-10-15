using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MusixSync.Networking;
namespace MusixSync.Mobile
{
    public static class NetBridge
    {
        public static bool IsAutoSyncing = false;
        public static bool IsManualSyncBrowsing = false;
        public static SyncDiscoveryClient SyncDiscoveryClient = new SyncDiscoveryClient();
        public static SyncDiscoveryClient AutoSyncClient;

    }
}