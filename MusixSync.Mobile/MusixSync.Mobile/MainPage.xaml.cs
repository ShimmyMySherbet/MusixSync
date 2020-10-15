using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using MusixSync.Mobile.Pages;
using MusixSync.Networking;
using MusixSync.Networking.Interfaces;
using Xamarin.Forms;

namespace MusixSync.Mobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        public string SyncingWith = "";

        public MainPage()
        {
            InitializeComponent();
            btnStart.Clicked += BtnStart_Clicked;
            cbAutoSync.CheckedChanged += CbAutoSync_CheckedChanged;
        }

        private void CbAutoSync_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (cbAutoSync.IsChecked)
            {
                NetBridge.AutoSyncClient = new Networking.SyncDiscoveryClient();
                NetBridge.AutoSyncClient.HostDiscovered += AutoSync_HostDiscovered;
                NetBridge.AutoSyncClient.StartSearch();
            }
            else
            {
                NetBridge.IsAutoSyncing = false;
                NetBridge.AutoSyncClient.StopSearch();
                NetBridge.AutoSyncClient.HostDiscovered -= AutoSync_HostDiscovered;
                NetBridge.AutoSyncClient = null;
            }
        }

        private void AutoSync_HostDiscovered(Networking.Models.SyncDevice host)
        {
            if (NetBridge.AutoSyncClient.AskSyncWithHost(host, out SyncClient client))
            {
                IFileWriter wr = MInterop.GetWriter();
                if (wr == null)
                {
                    client.Abort();
                    UITaskFactory.StartNew(() => cbAutoSync.IsChecked = false);
                }
                else
                {
                    client.FileWriter = wr;
                    SyncingWith = host.Host;
                    NetBridge.IsAutoSyncing = true;
                    client.SyncAborted += AutoSync_Aborted;
                    client.SyncComplete += Client_SyncComplete;
                }
            }
        }

        public void RestartAutoSync()
        {
            NetBridge.IsAutoSyncing = false;
            if (cbAutoSync.IsChecked)
            {
                NetBridge.AutoSyncClient.StopSearch();
                NetBridge.AutoSyncClient = new SyncDiscoveryClient();
                NetBridge.AutoSyncClient.HostDiscovered += AutoSync_HostDiscovered;
                NetBridge.AutoSyncClient.StartSearch();
            }
        }

        private void Client_SyncComplete(int FilesTransferred, int Errors)
        {
            NetBridge.IsAutoSyncing = false;
            SendNotification($"Auto Sync Complete", $"New Files: {FilesTransferred}");
            RestartAutoSync();
        }

        private void AutoSync_Aborted(Networking.Enums.SyncAbortReason Reason, string Message)
        {
            NetBridge.IsAutoSyncing = false;
            if (cbAutoSync.IsChecked)
            {
                RestartAutoSync();
            }
        }

        public static void SendNotification(string title, string message)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsInterface && typeof(INotificationManager).IsAssignableFrom(t))
                    {
                        INotificationManager mgr = (INotificationManager)Activator.CreateInstance(t);
                        mgr.ScheduleNotification(title, message);
                        return;
                    }
                }
            }
        }

        private async void BtnStart_Clicked(object sender, EventArgs e)
        {
            if (!NetBridge.IsAutoSyncing)
            {
                if (NetBridge.SyncDiscoveryClient != null)
                {
                    NetBridge.SyncDiscoveryClient.StopSearch();
                }
                NetBridge.SyncDiscoveryClient = new Networking.SyncDiscoveryClient();
                await stackmain.Navigation.PushAsync(new DiscoveryPage());
            }
            else
            {
                await DisplayAlert("Cannot Sync", $"A background auto-sync is active with {SyncingWith}.", "OK");
            }
        }
    }
}