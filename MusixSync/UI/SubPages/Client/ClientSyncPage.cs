using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusixSync.Networking;

namespace MusixSync.UI.SubPages.Client
{
    public partial class ClientSyncPage : UserControl
    {
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        public ClientSyncPage()
        {
            InitializeComponent();
        }

        public ClientSyncPage(SyncClient client)
        {
            client.OnSyncStatus += Client_OnSyncStatus;
            client.SyncAborted += Client_SyncAborted;
            client.SyncComplete += Client_SyncComplete;
            client.TransferComplete += Client_TransferComplete;
            client.DownloadsDecided += Client_DownloadsDecided;
            client.TransferStarted += Client_TransferStarted;
            lblSyncWith.Text = $"Syncing with: {client.HostName}";
            lblStatus.Text = "Waiting for host to start sync...";
            MusixSync.DiscoveryClient?.StopSearch();
            MusixSync.DiscoveryClient = null;
            rtbConsole.Text = "Waiting for remote host to accept sync...";
        }

        private async void Client_DownloadsDecided(List<string> Downloads)
        {
            await UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Requesting {Downloads.Count} file/s...\n");
                pbSyncProgress.Maximum = Downloads.Count;
                pbSyncProgress.Value = 0;
                pbSyncProgress.Step = 1;
            });
        }

        private async void Client_TransferStarted(string File, int completed, int intentMax)
        {
            await UITaskFactory.StartNew(() => {
                lblStatus.Text = $"Transfering file {File} ({completed}/{intentMax})";
                rtbConsole.AppendText($"Transfering file: {File} ({completed}/{intentMax})\n");
            });
        }

        private async void Client_TransferComplete(string File, int completed, int intentMax)
        {
          await  UITaskFactory.StartNew(() =>
            {
                 lblStatus.Text = $"Transfered file {File} ({completed}/{intentMax})";
                rtbConsole.AppendText($"Finished transfering file: {File} ({completed}/{intentMax})\n");

            });
        }

        private void Client_SyncComplete(int FilesTransferred, int Errors)
        {
            UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Sync completed with {Errors} errors.\n");
                MessageBox.Show(this, $"Sync completed.\nTransfers: {FilesTransferred}\nErrors: {Errors}", "Sync Complete");
                MusixSync.SyncPage.ShowPage<SyncHome>();
            });
        }

        private void Client_SyncAborted(Networking.Enums.SyncAbortReason Reason, string Message)
        {
            UITaskFactory.StartNew(() =>
            {
                rtbConsole.AppendText($"Sync Aborted: {Reason}\n");
                rtbConsole.AppendText(Message + "\n");
                MessageBox.Show(this, $"Sync aborted: {Message}");
                MusixSync.SyncPage.ShowPage<SyncHome>();
            });
        }

        private void Client_OnSyncStatus(bool Syncing)
        {
            UITaskFactory.StartNew(() =>
            {
                if (Syncing)
                {
                    rtbConsole.AppendText("Remote host accepted sync.\n");
                    rtbConsole.AppendText("Requesting remote server files list...\n");
                    lblStatus.Text = "Reading server files list...";
                }
                else
                {
                    rtbConsole.AppendText("Remote host declined/aborted the sync.\n");
                    MessageBox.Show(this, "The remote partner denied the sync.", "Sync failed");
                    MusixSync.SyncPage.ShowPage<SyncHome>();
                }
            });
        }
    }
}