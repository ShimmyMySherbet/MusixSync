using System;
using System.Windows.Forms;
using MusixSync.Networking;
using MusixSync.Networking.Models;

namespace MusixSync.UI
{
    public partial class SyncEntityControl : UserControl
    {
        public delegate void OnSyncArgs(SyncEntityControl instance);

        public event OnSyncArgs OnSyncRequested;

        public event OnSyncArgs OnSyncDenied;

        public SyncDevice Client;
        public SyncHost Host;

        public SyncEntityControl()
        {
            InitializeComponent();
        }

        public SyncEntityControl(string Host, string IP, SyncDevice client)
        {
            InitializeComponent();
            lblAddress.Text = IP;
            lblDeviceName.Text = Host;
            Client = client;
        }

        public SyncEntityControl(string Host, string IP, SyncHost host)
        {
            InitializeComponent();
            lblAddress.Text = "IP";
  
            lblDeviceName.Text = "Host";
            if (Host != null)
            {
                lblDeviceName.Text = Host;
            }
            if (IP != null)
            {
                lblAddress.Text = IP;
            }
            this.Host = host;
        }

        private void btnDenySync_Click(object sender, EventArgs e)
        {
            OnSyncDenied?.Invoke(this);
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            OnSyncRequested?.Invoke(this);
        }
    }
}