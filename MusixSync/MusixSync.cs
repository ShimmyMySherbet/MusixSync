using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Musix.Windows.API.Interfaces;
using Musix.Windows.API.Models;
using Musix.Windows.API.Themes;
using MusixSync.Networking;
using MusixSync.UI;

namespace MusixSync
{
    public class MusixSync : IMusixMenuItem, IMusixPlugin
    {
        public static SyncDiscoveryHost DiscoveryHost;
        public static SyncDiscoveryClient DiscoveryClient;

        public bool ShowWhenUnselected => true;

        public bool ShowWhenSelected => true;

        public string Name => "Sync";

        public System.Drawing.Image GetIcon(EStyle style)
        {
            if (style == EStyle.Blue)
            {
                return Assets.Musix_sync_Blue_v2;
            } else
            {
                return Assets.Musix_sync_Color_v2;
            }
        }
        public static SyncPage SyncPage = new SyncPage();
        public System.Windows.Forms.Control GetMenuControl()
        {
            return SyncPage;
        }

        public void OnDeselect()
        {
        }

        public void OnSelect()
        {
        }

        public void Load()
        {
            DiscoveryClient?.StopSearch();
            DiscoveryHost?.Stop();
        }

        public void Unload()
        {
            DiscoveryClient?.StopSearch();
            DiscoveryHost?.Stop();
            DiscoveryHost?.DisposeClient();
            if (DiscoveryHost != null && DiscoveryHost.ClientsWaiting != null)
            {
                DiscoveryHost.ClientsWaiting.ForEach(x => x.CancelSync());
            }
        }
    }
}
