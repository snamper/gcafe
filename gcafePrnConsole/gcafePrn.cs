using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gcafeSvc
{
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

        public string PrintChuPingCui(int orderId, int orderDetailId, int setmailId)
        {
            return "";
        }
    }
}
