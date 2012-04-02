using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace AtlasTCPSvcApp
{
    public partial class MainForm : Form
    {
        List<IPAddress> ACL;
        Server srv = null;
        IPAddress srvIP;
        int srvPort;
        bool firstHidden = false;

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public MainForm(string[] args)
        {
            InitializeComponent();

            trayIcon = new NotifyIcon();
            trayIcon.Text = this.Text;
            trayIcon.Icon = this.Icon;

            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Restore", OnRestore);
            trayMenu.MenuItems.Add("Close", OnExit);

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            ReadSettings();
            StartServer();
        }

        ~MainForm()
        {
            trayIcon.Dispose();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }

        private void OnRestore(object sender, EventArgs e)
        {
            Show();
        }

        public void WriteACL(List<IPAddress> ACL)
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | 
                IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            IsolatedStorageFileStream fwrite = new IsolatedStorageFileStream("settingsACL", FileMode.Create, iso);
            BinaryFormatter bfwrite = new BinaryFormatter();
            bfwrite.Serialize(fwrite, ACL);
            fwrite.Close();
        }

        public List<IPAddress> ReadACL()
        {
            List<IPAddress> ACL = new List<IPAddress>();
            IsolatedStorageFile iso = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | 
                IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            string[] fnms = iso.GetFileNames("*");
            if (((IList<string>)iso.GetFileNames("*")).Contains("settingsACL"))
            {
                IsolatedStorageFileStream fread = new IsolatedStorageFileStream("settingsACL", FileMode.Open, iso);
                BinaryFormatter bfread = new BinaryFormatter();
                ACL = (List<IPAddress>)bfread.Deserialize(fread);
                if (ACL == null)
                    ACL = new List<IPAddress>();
                fread.Close();
            }
            return ACL;
        }

        public void ReadSettings()
        {
            ACL = ReadACL();

            IsolatedStorageFile iso = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | 
                IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            string[] fnms = iso.GetFileNames("*");
            if (((IList<string>)iso.GetFileNames("*")).Contains("settingsMain"))
            {
                IsolatedStorageFileStream fread = new IsolatedStorageFileStream("settingsMain", FileMode.Open, iso);
                BinaryFormatter bfread = new BinaryFormatter();
                srvIP = (IPAddress)bfread.Deserialize(fread);
                if (srvIP == null)
                    IPAddress.TryParse("0.0.0.0", out srvIP);
                srvPort = (int)bfread.Deserialize(fread);
                if (srvPort == 0)
                    srvPort = 18000;
                fread.Close();
            }
            else
            {
                IPAddress.TryParse("0.0.0.0", out srvIP);
                srvPort = 18000;
            }


            updateSettingsInControls();
        }

        public void WriteSettings()
        {
            WriteACL(ACL);

            IsolatedStorageFile iso = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | 
                IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            IsolatedStorageFileStream fwrite = new IsolatedStorageFileStream("settingsMain", FileMode.Create, iso);
            BinaryFormatter bfwrite = new BinaryFormatter();
            bfwrite.Serialize(fwrite, srvIP);
            bfwrite.Serialize(fwrite, srvPort);
            fwrite.Close();
        }

        private delegate void AddLogMsgDelegate(string msg);

        public void AddLog(string msg)
        {
            if (this.logger.InvokeRequired)
                logger.Invoke(new AddLogMsgDelegate(this.AddLog), msg);
            else
                logger.AppendText(msg+"\r\n");
        }

        public void StartServer()
        {
            if (srv != null) return;
            srv = new Server(this, srvIP, srvPort, ACL);
            tmrChecker.Start();
            AddLog("Initialized. Waiting for incoming connections.");
        }

        public void StopServer()
        {
            if (srv == null) return;
            tmrChecker.Stop();
            srv.terminateListener();

            if (srv.isRestartNeeded())
            {
                AddLog("Restarting...");

                System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
                pi.FileName = Environment.GetCommandLineArgs()[0];
                System.Diagnostics.Process.Start(pi);
            }
            srv = null;
        }

        public void updateSettingsInControls()
        {
            listACL.Items.Clear();
            foreach (IPAddress ips in ACL)
            {
                listACL.Items.Add(ips.ToString());
            }
            edListenIP.Text = srvIP.ToString();
            edListenPort.Text = srvPort.ToString();
        }

        public void updateSettingsFromControls()
        {
            IPAddress ips;

            ACL.Clear();
            foreach (string s in listACL.Items)
            {
                if (IPAddress.TryParse(s, out ips))
                    ACL.Add(ips);
            }

            if (IPAddress.TryParse(edListenIP.Text, out ips))
                srvIP = ips;

            int port;
            if (int.TryParse(edListenPort.Text, out port))
                if (port > 0 && port < 65535)
                    srvPort = port;
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            logger.Clear();
        }

        private void btnAddACL_Click(object sender, EventArgs e)
        {
            string ips = edAddIP.Text;
            IPAddress ipt;
            if (!IPAddress.TryParse(ips, out ipt))
            {
                MessageBox.Show("Incorrect IP address. Cannot be added to ACL.");
                return;
            }

            ACL.Add(ipt);

            updateSettingsInControls();
        }

        private void btnDelACL_Click(object sender, EventArgs e)
        {
            if (listACL.SelectedIndex < 0 || listACL.SelectedIndex >= ACL.Count) return;

            ACL.RemoveAt(listACL.SelectedIndex);

            updateSettingsInControls();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            updateSettingsFromControls();
            WriteSettings();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopServer();
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }

        private void tmrChecker_Tick(object sender, EventArgs e)
        {
            if (srv == null) return;
            if (!srv.isListenerActive()) Close();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!firstHidden)
                Hide();
            firstHidden = true;
        }
    }
}
