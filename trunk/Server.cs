using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace AtlasTCPSvcApp
{
    class Server
    {
        TcpListener tcpListener;
        Thread listenerThread;
        bool listening;
        bool needRestart;
        List<IPAddress> ACL;
        MainForm form;

        public Server(MainForm mform, IPAddress ip, int port, List<IPAddress> aACL)
        {
            form = mform;
            ACL = aACL;
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
                IPEndPoint ipe = (IPEndPoint)client.Client.RemoteEndPoint;
                if (!ACL.Contains(ipe.Address))
                {
                    form.AddLog("Not authorized IP trying to access: " + ipe.ToString());
                    client.Client.Disconnect(false);
                    client.Close();
                    continue;
                }

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
            tcpListener.Stop();
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;

            form.AddLog("Client connected");

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
                        eng = new TranEngine();
                        if (!eng.initEngine())
                            sw.WriteLine("ERR:CANT_INIT");
                        else
                        {
                            engInited = true;
                            sw.WriteLine("OK");
                        }
                    }
                    else if (s.StartsWith("DIR:"))
                    {
                        if (!engInited)
                            sw.WriteLine("ERR:NEED_INIT");
                        else
                        {
                            s = s.Replace("DIR:", "");
                            s = s.ToUpper();
                            eng.setDirection(s);
                            sw.WriteLine("OK");
                        }
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

            form.AddLog("Client disconnected");

            tcpClient.Close();
            GC.Collect();
        }
    }
}
