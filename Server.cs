using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace AtlasTCPSvc
{
    class Server
    {
        TcpListener tcpListener;
        Thread listenerThread;
        bool listening;
        bool needRestart;
        int connectionCount;
        WaitTimerControl tmrControl;
        AtlasServiceControl svcControl;

        public Server(WaitTimerControl timerControl, AtlasServiceControl serviceControl, IPAddress ip, int port)
        {
            tmrControl = timerControl;
            svcControl = serviceControl;
            connectionCount = 0;
            listening = true;
            needRestart = false;
            tcpListener = new TcpListener(ip, port);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
        }

        ~Server()
        {
            terminateListener();
        }

        public bool isListenerActive()
        {
            return listening;
        }

        public bool isRestartNeeded()
        {
            return needRestart;
        }

        public void terminateListener()
        {
            lock (this)
            {
                listening = false;
            }
        }

        private void ListenForClients()
        {
            tcpListener.Start();
            while (listening)
            {
                if (!tcpListener.Pending())
                {
                    Thread.Sleep(500);
                    continue;
                }
                TcpClient client = tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
            tcpListener.Stop();
        }

        public void AuxTranslate(string src, out string dst)
        {
            TranEngine eng = null;
            dst = "ERR";
            try
            {
                    eng = new TranEngine();
                    eng.translatePar(src, out dst);
            }
            catch
            {
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;

            tmrControl(false);
            Interlocked.Increment(ref connectionCount);

            NetworkStream clientStream = tcpClient.GetStream();
            StreamReader sr = new StreamReader(clientStream, Encoding.ASCII);
            StreamWriter sw = new StreamWriter(clientStream, Encoding.ASCII);
            TranEngine eng = null;
            bool engInited = false;

            while (true)
            {
                if (engInited)
                {
                    if (eng.getSlippedCnt() > 20)
                    {
                        lock (this)
                        {
                            listening = false;
                            needRestart = true;
                        }
                        sw.WriteLine("ERR:INTERNAL_NEED_RESTART");
                        sw.Flush();
                        break;
                    }
                }
                try
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    if (s == "INIT")
                    {
                        engInited = true;
                        eng = new TranEngine();
                        sw.WriteLine("OK");
                    }
                    else if (s.StartsWith("DIR:"))
                    {
                        s = s.Replace("DIR:", "");
                        s = s.ToUpper();
                        eng.setDirection(s);
                        sw.WriteLine("OK");
                    }
                    else if (s.StartsWith("FIN"))
                    {
                        engInited = false;
                        sw.WriteLine("OK");
                        sw.Flush();
                        break;
                    }
                    else if (s.StartsWith("TR:"))
                    {
                        s = s.Replace("TR:", "");
                        string src = HttpUtility.UrlDecode(s, Encoding.UTF8);
                        if (src == null || src.Length < 1)
                        {
                            sw.WriteLine("ERR:NULL_STR_DECODED");
                        }
                        else
                        {
                            string dst;
                            if (!eng.translatePar(src, out dst))
                                sw.WriteLine("ERR:TRANS_FAILED");
                            else
                            {
                                dst = HttpUtility.UrlEncode(dst, Encoding.UTF8);
                                dst = "RES:" + dst;
                                sw.WriteLine(dst);
                            }
                        }
                    }
                    else
                        sw.WriteLine("ERR:NOT_RECOGNIZED");
                    sw.Flush();
                }
                catch
                {
                    break;
                }
            }

            Interlocked.Decrement(ref connectionCount);
            if (Interlocked.Equals(connectionCount, 0))
                tmrControl(true);

            if (needRestart)
                svcControl(AtlasServiceOpcode.Restart);

            tcpClient.Close();
            GC.Collect();
        }
    }
}
