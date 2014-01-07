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

using System.Data.OleDb;

namespace gcafeSvc
{
    public class gcafePrn : IgcafePrn, IDisposable
    {
        private bool _isStop = false;
        private Thread _thrPrint = null;
        //private PrintTaskMgr _printTaskMgr;

        public gcafePrn()
        {
            //_printTaskMgr = new PrintTaskMgr();
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

        public string PrintLiuTai(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                //_printTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintHuaDan, 1, -1));
                //System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new DoTask(Print), printTask);
                //_printTaskMgr.Print(new PrintTask(PrintTask.PrintType.PringHuaDan, 1, -1));
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintLiuTai, orderId, prnType, rePrint));
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        public string PrintHuaDan(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintHuaDan, orderId, prnType, rePrint));
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        public string PrintChuPing(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintChuPin, orderId, prnType, rePrint));
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        public string OrderPrint(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.OrderPrint, orderId, prnType, rePrint));
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        public string PrintChuPingCui(int orderId, int orderDetailId, int setmailId)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {

            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        public string OpenTable(string orderNum, string tableNum, string staffId, int customerNum)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("INSERT INTO orders(orderno, ordertime, custkind, personum, waiter, tableno, paid) VALUES('{0}', {4}, ' ', {1}, '{2}', '{3}', 0)",
                        GenOrderNo(), customerNum, staffId, tableNum, "{ fn NOW() }");

                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        private string GenOrderNo()
        {
            string strRtn = string.Empty;
            string fax = string.Empty;
            int orderNo = 0;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    // 取 sysinfo.fax
                    using (var cmd = new OleDbCommand("SELECT fax FROM sysinfo", conn))
                    {
                        fax = cmd.ExecuteScalar() as string;
                        fax = fax.Trim();
                    }

                    // 取 lastsn.orderno
                    while (true)
                    {
                        using (var cmd = new OleDbCommand("SELECT orderno FROM lastsn", conn))
                        {
                            orderNo = Int32.Parse(cmd.ExecuteScalar() as string) + 1;

                            // 更新lastsn.orderno
                            using (var cmd1 = new OleDbCommand(string.Format("UPDATE lastsn SET orderno = '{0}'", orderNo), conn))
                            {
                                cmd1.ExecuteNonQuery();
                            }

                            using (var cmd1 = new OleDbCommand(string.Format("SELECT orderno FROM orders WHERE orderno = '{0}'", orderNo), conn))
                            {
                                if (cmd1.ExecuteScalar() == null)
                                    break;
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("{0}, msg:{1}", Global.TraceMessage(), ex.Message));
            }

            strRtn = string.Format("{0}{1}{2:D2}{3:D2}{4:D4}", fax, System.DateTime.Now.Year.ToString().Substring(2, 2),
                System.DateTime.Now.Month, System.DateTime.Now.Day, orderNo);

            return strRtn;
        }


        public void Dispose()
        {
            _isStop = true;
            System.Diagnostics.Debug.WriteLine("============================================================");
            //_printTaskMgr.Dispose();
        }
    }
}
