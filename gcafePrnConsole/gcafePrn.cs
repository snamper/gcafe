using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;
using gcafePrnConsole;

namespace gcafeSvc
{
    public class gcafePrn : IgcafePrn, IDisposable
    {
        private bool _isStop = false;
        private Thread _thrPrint = null;
        private PrintTaskMgr _printTaskMgr;

        public gcafePrn()
        {
            _printTaskMgr = new PrintTaskMgr();
            //_thrPrint = new Thread(new ThreadStart(PrintThread));
            //_thrPrint.Start();
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
            Global.Logger.Trace(TraceMessage());

            try
            {
                _printTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintHuaDan, 1, -1));
                //System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new DoTask(Print), printTask);
                //_printTaskMgr.Print(new PrintTask(PrintTask.PrintType.PringHuaDan, 1, -1));

            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(TraceMessage());

            return "";
        }

        public string PrintHuaDan(int orderId, int prnType)
        {
            Global.Logger.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(TraceMessage());

            return "";
        }

        public string PrintChuPing(int orderId, int prnType)
        {
            Global.Logger.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(TraceMessage());

            return "";
        }

        public string PrintChuPingCui(int orderId, int orderDetailId, int setmailId)
        {
            Global.Logger.Trace(TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(TraceMessage());

            return "";
        }

        public void Dispose()
        {
            _isStop = true;
            System.Diagnostics.Debug.WriteLine("============================================================");
            _printTaskMgr.Dispose();
        }
    }
}
