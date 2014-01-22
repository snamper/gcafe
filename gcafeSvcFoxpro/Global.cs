using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace gcafeSvcFoxpro
{
    public class Global
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private static PrintTaskMgr _printTaskMgr = new PrintTaskMgr();

        public static PrintTaskMgr PrintTaskMgr
        {
            get { return _printTaskMgr; }
        }

        public static NLog.Logger Logger
        {
            get
            {
                return _log;
            }
        }

        public static string TraceMessage(
            string memberName = "",
            int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }

        public static string KitchenHuaDanPrinter
        {
            get
            {
                return Properties.Settings.Default.KitchenHuaDanPrinter;
            }
        }

        public static string BarHuaDanPrinter
        {
            get
            {
                return Properties.Settings.Default.BarHuaDanPrinter;
            }
        }

        public static string FoxproPath
        {
            get
            {
                return Properties.Settings.Default.FoxproPath;
            }
        }

        public static string FoxproPrntrPath
        {
            get
            {
                return Properties.Settings.Default.FoxproPrntrPath;
            }
        }

    }
}
