using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;

namespace gcafeSvc
{
    public class gcafePrn : IgcafePrn, IDisposable
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private bool _isStop = false;
        private Thread _thrPrint = null;

        public gcafePrn()
        {
            _thrPrint = new Thread(new ThreadStart(PrintThread));
            _thrPrint.Start();
        }

        private void PrintThread()
        {
            while (!_isStop)
            {
                Thread.Sleep(1000);
                System.Diagnostics.Debug.WriteLine(".");
            }

            System.Diagnostics.Debug.WriteLine("***************++++++++++++++++++++++++++++========================");
        }

        private static string TraceMessage(
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }


        public string PrintLiuTai(int orderId, int prnType)
        {
            _log.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return "";
        }

        public string PrintHuaDan(int orderId, bool printAll = false)
        {
            _log.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return "";
        }

        public string PrintHuaDan(int orderId, int prnType)
        {
            _log.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return "";
        }

        public string PrintChuPing(int orderId, int prnType)
        {
            _log.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return "";
        }

        public string PrintChuPingCui(int orderId, int orderDetailId, int setmailId)
        {
            _log.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return "";
        }

        public void Dispose()
        {
            _isStop = true;
            System.Diagnostics.Debug.WriteLine("============================================================");
        }
    }
}
