using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcafePrnConsole
{
    public class Global
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public static NLog.Logger Logger
        {
            get
            {
                return _log;
            }
        }
    }
}
