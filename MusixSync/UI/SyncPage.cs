using System;
using System.Linq;
using System.Windows.Forms;
using MusixSync.UI.SubPages;

namespace MusixSync.UI
{
    public partial class SyncPage : UserControl
    {
        public SyncPage()
        {
            InitializeComponent();
            Disposed += SyncPage_Disposed;
        }

        private void SyncPage_Disposed(object sender, EventArgs e)
        {
            MusixSync.DiscoveryHost?.Stop();
            MusixSync.DiscoveryClient?.StopSearch();
        }

        public void ShowPage(Control page)
        {
            if (pnPage.Controls.Contains(page))
            {
                pnPage.SuspendLayout();
                foreach (Control ct in pnPage.Controls)
                    ct.Visible = false;
                page.Dock = DockStyle.Fill;
                page.Visible = true;
                pnPage.ResumeLayout();
            }
            else
            {
                pnPage.SuspendLayout();
                foreach (Control ct in pnPage.Controls)
                    ct.Visible = false;
                page.Dock = DockStyle.Fill;
                pnPage.Controls.Add(page);
                page.Visible = true;
                pnPage.ResumeLayout();
            }
        }

        public Control ShowPage<T>() where T : Control
        {
            if (pnPage.Controls.OfType<T>().Count() > 0)
            {
                pnPage.SuspendLayout();
                Control existing = pnPage.Controls.OfType<T>().First();
                pnPage.SuspendLayout();
                foreach (Control ct in pnPage.Controls)
                    ct.Visible = false;
                existing.Visible = true;
                pnPage.ResumeLayout();
                return existing;
            }
            else
            {
                pnPage.SuspendLayout();
                Control NC = Activator.CreateInstance<T>();
                NC.Dock = DockStyle.Fill;
                foreach (Control ct in pnPage.Controls)
                    ct.Visible = false;
                pnPage.Controls.Add(NC);
                NC.Visible = true;
                pnPage.ResumeLayout();
                return NC;
            }
        }

        private void SyncPage_Load(object sender, EventArgs e)
        {
            ShowPage<SyncHome>();
        }
    }
}