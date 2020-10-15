using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MusixSync.Networking;
using MusixSync.Networking.Interfaces;
using MusixSync.Networking.Models;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace MusixSync.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SyncPage : ContentPage
    {
        public SyncClient Client;
        public int SyncFiles = 0;
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        public int FilesTransfered = 0;
        public SyncPage()
        {
            InitializeComponent();
            btnCancel.Clicked += Cancel_BypassClicked;
        }

        private void Cancel_BypassClicked(object sender, EventArgs e)
        {
            ExitToMainMenu();
        }

        public void ExitToMainMenu()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.Navigation.PopToRootAsync();
                Application.Current.MainPage.Navigation.PopModalAsync();
            });
        }

        public SyncPage(SyncClient client, string HostName)
        {
            InitializeComponent();
            Client = client;
            btnCancel.Clicked += BtnCancel_Clicked;
            lblTitle.Text = $"Syncing With: {HostName}";
            bool foundprov = false;
            foreach(Assembly A in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type T in A.GetTypes())
                {
                    if (typeof(IAdapterFileWriter).IsAssignableFrom(T) && !T.IsInterface && T != typeof(BasicFileWriter))
                    {//FileWriterAdapterBase
                        Client.FileWriter = new FileWriterAdapterBase((IAdapterFileWriter)Activator.CreateInstance(T));
                        foundprov = true;
                    }
                }
            }
            if (!foundprov)
            {
                DisplayAlert("ERROR", "Failed to find platform specific provider", "OK");
            }
            client.DownloadsDecided += Client_DownloadsDecided;
            client.OnSyncStatus += Client_OnSyncStatus;
            client.SyncAborted += Client_SyncAborted;
            client.SyncComplete += Client_SyncComplete;
            client.TransferComplete += Client_TransferComplete;
            client.TransferStarted += Client_TransferStarted;
            lblStatus.Text = "Waiting for remote server to start sync...";
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            Client.Abort();
            await DisplayAlert("Sync Aborted", $"The sync was aborted.\nTransfers: {FilesTransfered}", "OK");
            ExitToMainMenu();
        }

        private void Client_TransferStarted(string File, int completed, int intentMax)
        {
            UITaskFactory.StartNew(() =>
            {
                lblStatus.Text = $"Transferring file {File} ({completed}/{intentMax})";
            });
        }

        private void Client_TransferComplete(string File, int completed, int intentMax)
        {
            UITaskFactory.StartNew(() =>
            {
                FilesTransfered++;
                pbProg.Progress = (completed / intentMax);
                lblStatus.Text = $"Finished transferring file {File} ({completed}/{intentMax})";
            });
        }

        private void Client_SyncComplete(int FilesTransferred, int Errors)
        {
            UITaskFactory.StartNew(async () =>
            {
                await DisplayAlert("Sync Complete", $"Files Transferred: {FilesTransferred}\nErrors: {Errors}", "OK");
                ExitToMainMenu();
            });
        }

        private void Client_SyncAborted(Networking.Enums.SyncAbortReason Reason, string Message)
        {
            if (Reason == Networking.Enums.SyncAbortReason.SyncWasCanceled) return;
            UITaskFactory.StartNew(async () =>
            {
                await DisplayAlert("Sync Aborted", $"The sync was aborted. {Message}", "OK");
                ExitToMainMenu();
            });
        }

        private void Client_OnSyncStatus(bool Syncing)
        {
            UITaskFactory.StartNew(async () =>
            {
                if (!Syncing)
                {
                    await DisplayAlert("Failed to sync", "The remote server aborted the sync.", "OK");
                    ExitToMainMenu();
                }
                else
                {
                    lblStatus.Text = "Remote server accepted sync.";
                }
            });
        }

        private void Client_DownloadsDecided(List<string> Downloads)
        {
            UITaskFactory.StartNew(() =>
            {
                SyncFiles = Downloads.Count;
                lblStatus.Text = $"Requesting {Downloads.Count} files from server...";
            });
        }
    }
}