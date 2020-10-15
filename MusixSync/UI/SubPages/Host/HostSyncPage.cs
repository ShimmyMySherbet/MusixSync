using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusixSync.Networking;

namespace MusixSync.UI.SubPages.Host
{
    public partial class HostSyncPage : UserControl
    {
        public SyncHost Host;
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        public HostSyncPage()
        {
            InitializeComponent();
        }

        public HostSyncPage(SyncHost host)
        {
            InitializeComponent();
            Host = host;
            lblSyncWith.Text = $"Syncing With: {host.HostName} @ {host.Address}";
            host.EnableProxyEvents = false;
            host.DownloadIntentRecieved += Host_DownloadIntentRecieved;
            host.SyncAborted += Host_SyncAborted;
            host.SyncComplete += Host_SyncComplete;
            host.TransferComplete += Host_TransferComplete;
            host.TransferStarted += Host_TransferStarted;
            host.ProceedSync(true);
        }

        private async void Host_TransferStarted(string File, int completed, int intentMax)
        {
          await  UITaskFactory.StartNew(() =>
            {
                lblStatus.Text = $"Transfering file: {File} ({completed}/{intentMax})";
                rtbConsole.AppendText($"Transfering file: {File} ({completed}/{intentMax})\n");
            });
        }

        private async void Host_TransferComplete(string File, int completed, int intentMax)
        {
            await UITaskFactory.StartNew(() =>
            {
                pbSyncProgress.Value = completed;
                lblStatus.Text = $"Finished transfering file: {File} ({completed}/{intentMax})";
                rtbConsole.AppendText($"Finished transfering file: {File} ({completed}/{intentMax})\n");
            });
        }

        private void Host_SyncComplete(int FilesTransferred, int Errors)
        {
            UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Sync completed with {Errors} errors.\n");
                MessageBox.Show(this, $"Sync completed with {Errors} errors.", "Sync Complete");
                MusixSync.SyncPage.ShowPage<SyncHome>();
                MusixSync.DiscoveryHost.IsDiscoverable = false;
                MusixSync.DiscoveryHost.Stop();
                Dispose();
            });
        }

        private void Host_SyncAborted(Networking.Enums.SyncAbortReason Reason, string Message)
        {
            UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Sync Aborted: {Reason}\n");
                rtbConsole.AppendText(Message + "\n");
                MessageBox.Show(this, $"Sync Aborted; {Reason}\n{Message}", "Sync Failed");
                MusixSync.SyncPage.ShowPage<SyncHome>();
                MusixSync.DiscoveryHost.IsDiscoverable = false;
                MusixSync.DiscoveryHost.Stop();
                Dispose();
            });
        }

        private void Host_DownloadIntentRecieved(List<string> Downloads)
        {
            UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Client has requested {Downloads.Count} file/s...\n");
                pbSyncProgress.Maximum = Downloads.Count;
                pbSyncProgress.Step = 1;
                lblStatus.Text = $"Client requested {Downloads.Count} file/s...";
            });
        }

        private void HostSyncPage_Load(object sender, EventArgs e)
        {
            rtbConsole.Text = "Starting Sync...";
            lblStatus.Text = "Starting Sync...";
        }
    }
}