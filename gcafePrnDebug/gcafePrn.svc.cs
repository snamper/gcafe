using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gcafePrnDebug
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "gcafePrn" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select gcafePrn.svc or gcafePrn.svc.cs at the Solution Explorer and start debugging.
    public class gcafePrn : IgcafePrn
    {
        public string PrintLiuTai(int orderId, int prnType)
        {
            return "";
        }

        public string PrintHuaDan(int orderId, bool printAll = false)
        {
            return "";
        }

        public string PrintHuaDan(int orderId, int prnType)
        {
            return "";
        }

        public string PrintChuPing(int orderId, int prnType)
        {
            return "";
        }

        public string PrintChuPing(int orderId, int orderDetailId, int setmailId)
        {
            return "";
        }
    }
}
