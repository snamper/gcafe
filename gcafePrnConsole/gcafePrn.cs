using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;

namespace gcafeSvc
{
    public class gcafePrn : IgcafePrn
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

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
    }
}
