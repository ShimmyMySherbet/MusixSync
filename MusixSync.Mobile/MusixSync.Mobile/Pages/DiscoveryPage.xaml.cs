using System.Threading.Tasks;
using MusixSync.Networking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MusixSync.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoveryPage : ContentPage
    {
        public static DiscoveryPage Instance;
        public TaskFactory UITaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        public DiscoveryPage()
        {
            Instance = this;
            InitializeComponent();
            NetBridge.SyncDiscoveryClient.HostDiscovered += SyncDiscoveryClient_HostDiscovered;
            NetBridge.SyncDiscoveryClient.StartSearch();
            btnTmp.Clicked += BtnTmp_Clicked;
        }

        private async void BtnTmp_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new SyncPage());
        }

        private void SyncDiscoveryClient_HostDiscovered(Networking.Models.SyncDevice host)
        {
            UITaskFactory.StartNew(() =>
            {
                Button btnN = new Button()
                {
                    Text = host.Host,
                    FontSize = 20,
                    Padding = new Thickness(1)
                };
                btnN.Clicked += async (a, b) =>
                {
                    if (NetBridge.SyncDiscoveryClient.AskSyncWithHost(host, out SyncClient client))
                    {
                        if (client.IsAborted)
                        {
                            await DisplayAlert("Failed to connect", $"Remote server aborted unexpectedly", "OK");
                        }
                        else
                        {
                            await Navigation.PushModalAsync(new SyncPage(client, host.Host));
                        }
                    }
                    else
                    {
                        await DisplayAlert("Failed to connect", $"Failed to connect to {host.Host} @ {host.IP}", "OK");
                    }
                };

                StackMain.Children.Add(btnN);

            });
        }
    }
}