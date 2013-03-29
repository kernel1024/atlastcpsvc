using System;
using System.Collections.Generic;
using System.Text;
using AtlasServer_ProV14;
using Atlct_ProV14;

namespace AtlasTCPSvc
{
    class TranEngine
    {
        // Multithreaded blocking interface to single-threaded ATLAS

        static private TextTransClass engine = null;
        static short oldTranDirection = 0;

        static readonly object slipLocker = new object();
        static readonly object engineLocker = new object();

        static int slippedCnt = 0;

        short tranDirection = 0;

        public int getSlippedCnt()
        {
            lock (slipLocker)
            {
                return slippedCnt;
            }
        }

        static private void shutdownEngine()
        {
            lock (engineLocker)
            {
                try
                {
                    engine.CallTerm();
                }
                catch { }
            }
        }

        static public bool initEngine()
        {
            bool initSuccess = false;
            lock (engineLocker)
            {
                engine = new TextTransClass();
                initSuccess = (engine.CallInit() == 0);
                if (!initSuccess)
                    engine = null;
            }
            return initSuccess;
        }

        public void setDirection(string lang)
        {
            tranDirection = (short)TransDirection.atTransModeDirectionAuto;
            if (lang == "EJ") tranDirection = (short)TransDirection.atTransModeDirectionEj;
            if (lang == "JE") tranDirection = (short)TransDirection.atTransModeDirectionJe;
        }

        public bool translatePar(string src, out string res)
        {
            res = "";
            object dic = null;
            int errc = 0;
            char[] spcs = { '\r', '\n', ' ' };
            string s = "";

            lock (engineLocker)
            {
                try
                {
                    if (engine != null)
                    {
                        if (oldTranDirection != tranDirection)
                            engine.SetMode_Direction(tranDirection);
                        oldTranDirection = tranDirection;
                        s = engine.CallTextTranslate(src, ref dic, ref errc);
                    }
                    if (s == null) s = "";
                    else s = s.Trim(spcs);
                }
                catch
                {
                    s = "";
                }
            }

            if (s.Trim() == src.Trim())
            {
                lock (slipLocker)
                {
                    slippedCnt++;
                }
            }
            else
            {
                lock (slipLocker)
                {
                    slippedCnt = 0;
                }
            }
            res = s;
            return true;
        }
    }
}
