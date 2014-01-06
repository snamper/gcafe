using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace gcafePrnConsole
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
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }


        public static int BranchId
        {
            get
            {
                return Int32.Parse(ConfigurationSettings.AppSettings["BranchID"]);
            }
        }

        public static string KitchenHuaDanPrinter
        {
            get
            {
                return ConfigurationSettings.AppSettings["厨房划单打印机"];
            }
        }

        public static string BarHuaDanPrinter
        {
            get
            {
                return ConfigurationSettings.AppSettings["酒吧划单打印机"];
            }
        }

        public static string FoxproPath
        {
            get
            {
                return gcafePrnConsole.Properties.Settings.Default.FoxproPath;
            }
        }

    }
}
