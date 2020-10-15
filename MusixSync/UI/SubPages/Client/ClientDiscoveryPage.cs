using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusixSync.Networking;

namespace MusixSync.UI.SubPages.Client
{
    public partial class ClientDiscoveryPage : UserControl
    {
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        public ClientDiscoveryPage()
        {
            InitializeComponent();
            if (MusixSync.DiscoveryClient != null)
            {
                MusixSync.DiscoveryClient.StopSearch();
                MusixSync.DiscoveryClient = new Networking.SyncDiscoveryClient();
            }
            MusixSync.DiscoveryClient = new SyncDiscoveryClient();
            MusixSync.DiscoveryClient.HostDiscovered += DiscoveryClient_HostDiscovered;
            MusixSync.DiscoveryClient.StartSearch();
        }

        private void DiscoveryClient_HostDiscovered(Networking.Models.SyncDevice host)
        {
            SyncEntityControl ct = new SyncEntityControl(host.Host, host.Address.ToString(), host);
            ct.OnSyncDenied += Ct_OnSyncDenied;
            ct.OnSyncRequested += Ct_OnSyncRequested;
            MusixSync.SyncPage.ShowPage(ct);
            Dispose();
        }

        private void Ct_OnSyncRequested(SyncEntityControl instance)
        {
            if (MusixSync.DiscoveryClient.AskSyncWithHost(instance.Client, out SyncClient client))
            {
                MusixSync.DiscoveryClient.StopSearch();
            } else
            {
                MessageBox.Show(this, "Failed to connect to remote client.", "Failed to initiate sync");
            }
        }

        private void Ct_OnSyncDenied(SyncEntityControl instance)
        {
            UITaskFactory.StartNew(() =>
            {
                if (flowDevices.Controls.Contains(instance))
                {
                    flowDevices.Controls.Remove(instance);
                }
            });
        }

        private void ClientDiscoveryPage_Load(object sender, EventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MusixSync.DiscoveryClient?.StopSearch();
            MusixSync.DiscoveryClient = null;
            MusixSync.SyncPage.ShowPage<SyncHome>();
            Dispose();
        }
    }
}
