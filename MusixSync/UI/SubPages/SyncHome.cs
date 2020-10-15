using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Instrumentation;
using MusixSync.UI.SubPages.Host;
using MusixSync.UI.SubPages.Client;

namespace MusixSync.UI.SubPages
{
    public partial class SyncHome : UserControl
    {
        public SyncHome()
        {
            InitializeComponent();
            SizeChanged += SyncHome_SizeChanged;
            CentreControl(btnSyncTo);
            CentreControl(btnSyncFrom);
        }

        private void SyncHome_SizeChanged(object sender, EventArgs e)
        {
            splitC.SplitterDistance = splitC.Width / 2;
            Console.WriteLine("Size Update");
            CentreControl(btnSyncTo);
            CentreControl(btnSyncFrom);
        }


        public void CentreControl(Control control)
        {
            int cWidth = control.Width;
            int offset = control.Parent. Width - cWidth;
            Point rPos = control.Location;
            rPos.X = (int)(offset / 2);


            int cHeight = control.Height;
            int hoffset = control.Parent.Height - cHeight;
            rPos.Y = (int)(hoffset / 2.5);


            control.Location = rPos;
        }

        private void btnSyncTo_Click(object sender, EventArgs e)
        {
            MusixSync.SyncPage.ShowPage(new HostDiscoveryScreen());
        }

        private void btnSyncFrom_Click(object sender, EventArgs e)
        {
            MusixSync.SyncPage.ShowPage(new ClientDiscoveryPage());

        }
    }
}
