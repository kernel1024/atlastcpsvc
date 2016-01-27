using System;
using System.Net;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;

namespace AtlasTCPSvc
{
    public partial class MainService : ServiceBase
    {
        Server server;
        Timer tmrTranWakeup;
        bool isWaiting;

        public MainService()
        {
            InitializeComponent();
            TimerCallback tcb = this.tmrTranWakeup_Tick;
            tmrTranWakeup = new Timer(tcb, null, 0, 60000);
            isWaiting = true;
        }

        protected override void OnStart(string[] args)
        {
            string certFilename = "";
            try
            {
                certFilename = AppDomain.CurrentDomain.BaseDirectory;
                if (!certFilename.EndsWith("\\")) certFilename += "\\";
                string[] files = Directory.GetFiles(certFilename, "*.pfx");
                if (files.Length > 0)
                    certFilename = files.First();
                else
                    certFilename = "";
            } catch (Exception e) {
                certFilename = "";
                this.EventLog.WriteEntry("Catched exception while SSL certificate searching: "+e.Message,EventLogEntryType.Error);
            }

            if (certFilename.Length == 0)
            {
                this.EventLog.WriteEntry("Unable to find SSL certificate in " + AppDomain.CurrentDomain.BaseDirectory + " directory, terminating.",
                    EventLogEntryType.Warning);
                Stop();
                return;
            }

            int srvPort = 18000;
            try
            {
                if (args.Count() > 1)
                    srvPort = Convert.ToInt32(args[1]);

                if (!AtlasTCPSvc.TranEngine.initEngine())
                    throw new AtlasException("ATLAS not initialized.");

                StartServer(srvPort, certFilename);
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                ExitCode = 1;
                Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                StopServer();
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                ExitCode = 1;
            }
            if (ExitCode != 0)
                Environment.Exit(ExitCode);
        }

        public void StartServer(int srvPort, string certFilename)
        {
            if (server != null) return;
            IPAddress srvIP;
            IPAddress.TryParse("0.0.0.0", out srvIP);
            server = new Server(messageLogger, tokenAuthenticator, tmrControl, svcControl, srvIP, srvPort, certFilename);
        }

        public int messageLogger(string msg)
        {
            this.EventLog.WriteEntry(msg, EventLogEntryType.Error);
            return 0;
        }

        public bool tokenAuthenticator(string token)
        {
            RegistryKey pkey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\AtlasTCPSvc");
            List<string> tokenList = new List<string>((string[])pkey.GetValue("TokenList", new string[] { }));
            pkey.Close();

            if (tokenList.Contains(token))
                return true;

            return false;
        }

        public void StopServer()
        {
            if (server == null) return;
            isWaiting = false;
            server.terminateListener();

            if (server.isRestartNeeded())
            {
                this.EventLog.WriteEntry("Need to restart ATLAS after slipping", EventLogEntryType.Information);
                ExitCode = 2;
            }
            else
                ExitCode = 0;

            server = null;
        }

        public void svcControl(AtlasServiceOpcode opcode)
        {
            switch (opcode)
            {
                case AtlasServiceOpcode.Restart:
                    {
                        Stop();
                        break;
                    }
            }
        }

        public void tmrControl(bool start)
        {
            if (start)
                tmrTranWakeupStart();
            else
                tmrTranWakeupStop();
        }

        public void tmrTranWakeup_Tick(object stateInfo)
        {
            if (!isWaiting) return;
            if (server == null) return;
            if (!server.isRestartNeeded())
            {
                string src = "新幹線は、JRグループの東日本旅客鉄道、東海旅客鉄道、西日本旅客鉄道、九州旅客鉄道が運営する日本の高速鉄道である。";
                string dst;
                server.AuxTranslate(src, out dst);
            }
        }

        public void tmrTranWakeupStart()
        {
            isWaiting = true;
        }

        public void tmrTranWakeupStop()
        {
            isWaiting = false;
        }
    }

    public enum AtlasServiceOpcode { None, Restart };
    delegate void WaitTimerControl(bool start);
    delegate void AtlasServiceControl(AtlasServiceOpcode opcode);

    public class AtlasException : System.Exception
    {
        public AtlasException()
        {
        }

        public AtlasException(string message)
            : base(message)
        {
        }

        public AtlasException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


}
