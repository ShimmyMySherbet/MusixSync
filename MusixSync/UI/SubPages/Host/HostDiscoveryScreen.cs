using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusixSync.Networking.Models;

namespace MusixSync.UI.SubPages.Host
{
    public partial class HostDiscoveryScreen : UserControl
    {
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        public HostDiscoveryScreen()
        {
            InitializeComponent();
        }

        private void HostDiscoveryScreen_Load(object sender, EventArgs e)
        {
            if (MusixSync.DiscoveryHost == null)
            {
                MusixSync.DiscoveryHost = new Networking.SyncDiscoveryHost();
            }
            MusixSync.DiscoveryHost.OnClientConnect += DiscoveryHost_OnClientConnect;
            MusixSync.DiscoveryHost.IsDiscoverable = true;
            MusixSync.DiscoveryHost.Start();
        }

        private void DiscoveryHost_OnClientConnect(Networking.SyncHost client)
        {
            if (client.IsAborted) return;
            client.FileProvider = new SimpleFileProvider("Music");
            client.SyncAbortedProxy += Client_SyncAbortedProxy;
            client.EnableProxyEvents = true;
            UITaskFactory.StartNew(() =>
            {
                SyncEntityControl ent = new SyncEntityControl(client.HostName, client.Address.ToString(), client);
                ent.OnSyncDenied += Ent_OnSyncDenied;
                ent.OnSyncRequested += Ent_OnSyncRequested;
                flowDevices.Controls.Add(ent);
            });
        }

        private void Client_SyncAbortedProxy(Networking.Enums.SyncAbortReason Reason, string Message, Networking.SyncHost host)
        {
            UITaskFactory.StartNew(() =>
            {
                SyncEntityControl[] CT = flowDevices.Controls.OfType<SyncEntityControl>().Where(x => x.Host == host).ToArray();
                if (CT.Length > 0)
                {
                    SyncEntityControl instance = CT[0];
                    flowDevices.SuspendLayout();
                    flowDevices.Controls.Remove(instance);
                    flowDevices.ResumeLayout();
                }
            });
        }


        private async void Ent_OnSyncRequested(SyncEntityControl instance)
        {
            Console.WriteLine("pre send sync");
            await UITaskFactory.StartNew(() =>
            {
                Console.WriteLine("trigger sync with client");
                MusixSync.DiscoveryHost.SyncWithClient(instance.Host);
                Console.WriteLine("aft sync with client");
                HostSyncPage NP = new HostSyncPage(instance.Host);
                MusixSync.SyncPage.ShowPage(NP);
                Dispose();

            });
            Console.WriteLine("asend sync");
        }

        private void Ent_OnSyncDenied(SyncEntityControl instance)
        {
            UITaskFactory.StartNew(() =>
            {
                if (flowDevices.Controls.Contains(instance))
                {
                    flowDevices.SuspendLayout();
                    flowDevices.Controls.Remove(instance);
                    flowDevices.ResumeLayout();
                }
                instance.Host?.ProceedSync(false);
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MusixSync.DiscoveryHost.IsDiscoverable = false;
            MusixSync.DiscoveryHost.Stop();
            MusixSync.SyncPage.ShowPage<SyncHome>();
            Dispose();
        }
    }
}