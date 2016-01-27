using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

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
        X509Certificate2 sslCert = null;
        Func<string, int> logMessage;
        Func<string, bool> tokenAuth;

        public Server(Func<string, int> messageLogger, Func<string, bool> tokenAuthenticator, WaitTimerControl timerControl,
            AtlasServiceControl serviceControl, IPAddress ip, int port, string sslCertFile)
        {
            try
            {
                sslCert = new X509Certificate2(sslCertFile,"");
            }
            catch (Exception e)
            {
                messageLogger("Unable to load SSL certificate " + sslCertFile + ". " + e.Message);
            }

            logMessage = messageLogger;
            tokenAuth = tokenAuthenticator;
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
            SslStream clientStream = new SslStream(tcpClient.GetStream(), false);
            //NetworkStream clientStream = tcpClient.GetStream();

            tmrControl(false);
            Interlocked.Increment(ref connectionCount);

            try
            {
                clientStream.AuthenticateAsServer(sslCert, false, SslProtocols.Default, true);

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
                        if (s.StartsWith("INIT:"))
                        {
                            s = s.Replace("INIT:", "");
                            if (tokenAuth(s))
                            {
                                engInited = true;
                                eng = new TranEngine();
                                sw.WriteLine("OK");
                            }
                            else
                            {
                                sw.WriteLine("ERR:NOT_AUTHORIZED");
                                sw.Flush();
                                break;
                            }
                        }
                        else if (s.StartsWith("DIR:") && engInited)
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
                        else if (s.StartsWith("TR:") && engInited)
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
                        {
                            sw.WriteLine("ERR:NOT_RECOGNIZED");
                            sw.Flush();
                            break;
                        }
                        sw.Flush();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                string msg = "SSL auth exception - " + e.Message;
                if (e.InnerException!=null)
                    msg+="\r\n Inner exception: "+e.InnerException.Message;
                logMessage(msg);
            }

            Interlocked.Decrement(ref connectionCount);
            if (Interlocked.Equals(connectionCount, 0))
                tmrControl(true);

            if (needRestart)
                svcControl(AtlasServiceOpcode.Restart);
            
            clientStream.Flush();
            clientStream.Close();
            tcpClient.Close();
            GC.Collect();
        }
    }
}
