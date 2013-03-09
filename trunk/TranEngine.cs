using System;
using System.Collections.Generic;
using System.Text;
using AtlasServer_ProV14;
using Atlct_ProV14;

namespace AtlasTCPSvcApp
{
    class TranEngine
    {
        // Multithreaded blocking interface to single-threaded ATLAS

        static private TextTransClass engine;

        static readonly object slipLocker = new object();
        static readonly object refLocker = new object();
        static readonly object engineLocker = new object();

        static int slippedCnt = 0;
        static int refCnt = 0;

        short tranDirection = 0;

        public TranEngine()
        {
            lock (refLocker)
            {
                if (refCnt == 0)  // Initialize engine if this is first instance
                {
                    lock (engineLocker)
                    {
                        engine = null;
                    }
                }
            }
        }

        ~TranEngine()
        {
            shutdownEngine();
        }

        public int getSlippedCnt()
        {
            lock (slipLocker)
            {
                return slippedCnt;
            }
        }

        private void shutdownEngine()
        {
            lock (refLocker)
            {
                refCnt--;
                if (refCnt > 0) return;
            }
            // reference counter == 0, close engine
            lock (engineLocker)
            {
                try
                {
                    engine.CallTerm();
                }
                catch { }
            }
        }

        public bool initEngine()
        {
            bool needInit = false;
            bool initSuccess = false;

            lock (refLocker)
            {
                needInit = (refCnt == 0);
            }
            lock (engineLocker)
            {
                if (needInit)
                {
                    engine = new TextTransClass();
                    initSuccess = (engine.CallInit() == 0);
                }
            }
            lock (refLocker)
            {
                if (refCnt > 0 || initSuccess) refCnt++;
                return (refCnt > 0);
            }
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

            lock (refLocker)
            {
                if (refCnt == 0) return false;
            }
            lock (engineLocker)
            {
                try
                {
                    engine.SetMode_Direction(tranDirection);
                    s = engine.CallTextTranslate(src, ref dic, ref errc);
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
