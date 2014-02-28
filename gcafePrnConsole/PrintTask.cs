using System;
using System.Windows;
using System.Printing;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Data;
using System.Data.Entity;
using System.Data.OleDb; 

namespace gcafePrnConsole
{
    public class PrintTask
    {
        public enum PrintType { OrderPrint, PrintLiuTai, PrintChuPin, PrintHuaDan };

        public PrintTask(PrintType type, int orderId, int prnType, bool rePrint = false, int orderDetailId = -1, int setmealId = -1)
        {
            Type = type;
            PrnType = prnType;
            OrderId = orderId;
            OrderDetailId = orderDetailId;
            RePrint = rePrint;
            SetmealId = setmealId;
            IsUrge = !(OrderDetailId == -1 && SetmealId == -1);
        }

        public PrintType Type { get; private set; }
        public int OrderId { get; private set; }
        public int PrnType { get; private set; }
        public int OrderDetailId { get; private set; }
        public int SetmealId { get; private set; }
        public bool IsUrge { get; set; }
        public bool RePrint { get; set; }
    }

    public class PrintTaskMgr : Queue<PrintTask>, IDisposable
    {
        Thread _thrTask;
        bool _isStop = false;
        Task _taskListScanner;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mutex _mutex = new Mutex();
        Mutex _mutexFoxpro = new Mutex();

        public PrintTaskMgr()
        {
            CancellationToken token = tokenSource.Token;
            _thrTask = new Thread(new ThreadStart(ThreadTask));
            _thrTask.SetApartmentState(ApartmentState.STA);
            _thrTask.IsBackground = true;
            _thrTask.Start();
            //_taskListScanner = Task.Factory.StartNew(async () =>
            //    {
            //        while (!_isStop)
            //        {
            //            if (Count > 0)
            //            {
            //                _mutex.WaitOne();
            //                PrintTask printTask = this.Dequeue();
            //                _mutex.ReleaseMutex();

            //                await Print(printTask);
            //            }

            //            Thread.Sleep(1000);
            //            System.Diagnostics.Debug.WriteLine(".");
            //        }

            //        System.Diagnostics.Debug.WriteLine("task finished");
            //    }, token);
        }

        void ThreadTask()
        {
            while (!_isStop)
            {
                if (Count > 0)
                {
                    _mutex.WaitOne();
                    PrintTask printTask = this.Dequeue();
                    _mutex.ReleaseMutex();

                    System.Diagnostics.Debug.WriteLine("==== {0}, {1} ====", printTask.Type, Count);

                    switch (printTask.Type)
                    {
                        case PrintTask.PrintType.PrintHuaDan:
                            PrintHuaDan(printTask.OrderId, printTask.PrnType);
                            break;

                        case PrintTask.PrintType.PrintChuPin:
                            PrintChuPin(printTask.OrderId, printTask.PrnType);
                            break;

                        case PrintTask.PrintType.PrintLiuTai:
                            PrintLiuTai(printTask.OrderId, printTask.PrnType);
                            break;

                        case PrintTask.PrintType.OrderPrint:
                            //await PrintChuPin(printTask.OrderId, printTask.PrnType);
                            //List<order_detail> orderDetails = await GetOrderDetailList(printTask.OrderId, printTask.PrnType);
                            //await PrintChuPin(orderDetails, printTask.RePrint);
                            break;
                    }

                    //await Print(printTask);
                }

                Thread.Sleep(1000);
            }

            int i = 0;
        }


        string PrintHuaDan(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

#if FOXPRO
            try
            {
                string orderNo = GetFoxproOrderNo(orderId);
                if (string.IsNullOrEmpty(orderNo))
                {
                    Global.Logger.Debug(string.Format("PrintHuaDan中orderno出错:{0}", orderId));
                    return string.Format("PrintHuaDan中orderno出错:{0}", orderId);
                }

                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Empty;
                    string orderTime = null;
                    string tableNum = null;
                    string waiter = null;

                    #region 打印第几次，如果全打不执行
                    // 打印第几次，如果全打不执行
                    if (prnType != 0)
                    {
                        sql = string.Format("SELECT ordertime FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNo);
                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            OleDbDataReader reader = cmd.ExecuteReader();
                            int cnt = 1;
                            DateTime prevTime = DateTime.Now;
                            while (reader.Read())
                            {
                                DateTime ot = reader.GetDateTime(0);
                                if (prevTime != ot)
                                {
                                    orderTime = string.Format("{0}^{1}{2}", "{", ot.ToString("u"), "}");
                                    if (cnt == prnType)
                                        break;

                                    cnt++;

                                    prevTime = ot;
                                }
                            }
                        }
                    }
                    #endregion 打印第几次，如果全打不执行

                    if (orderTime == null)
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter FROM poh WHERE (orderno = '{0}') AND (department = '11') ORDER BY serial", orderNo);
                    else
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter FROM poh WHERE (orderno = '{0}') AND (ordertime = {1}) AND (department = '11') ORDER BY serial", orderNo, orderTime);

                    List<order_detail> orderDetails = new List<order_detail>();
                    #region 填入orderDetails
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        string prevSerial = string.Empty;
                        order_detail orderDetail = null;
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string serial = reader.GetString(0).Trim();
                            string prodName = reader.GetString(1).Trim();
                            int quantity = reader.GetInt32(2);
                            string prnGrp = reader.GetString(3).Trim();
                            string remark1 = reader.GetString(4).Trim();
                            string remark2 = reader.GetString(5).Trim();
                            if (tableNum == null)
                                tableNum = reader.GetString(6).Trim();
                            if (waiter == null)
                                waiter = reader.GetString(7).Trim();

                            // 以serial来区分不同的项目，原因是套餐项目时，如果套餐名相同时
                            // 区分不同的套餐内容
                            if (serial != prevSerial)
                            {
                                if (orderDetail != null)
                                {
                                    orderDetails.Add(orderDetail);
                                }

                                orderDetail = new order_detail();
                                if (string.IsNullOrEmpty(remark1))
                                {
                                    // 这个项目不是套餐
                                    orderDetail.menu = new menu() { name = prodName };
                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            orderDetail.order_detail_method.Add(
                                                new order_detail_method()
                                                {
                                                    method = new method() { name = method }
                                                });
                                    }
                                }
                                else
                                {
                                    // 这个项目是套餐
                                    orderDetail.menu = new menu() { name = remark1 };
                                    orderDetail.order_detail_setmeal.Add(new order_detail_setmeal()
                                    {
                                        menu = new menu() { name = prodName }
                                    });

                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            orderDetail.order_detail_setmeal.Last().order_detail_method.Add(
                                                new order_detail_method()
                                                {
                                                    method = new method() { name = method }
                                                });
                                    }
                                }
                                orderDetail.quantity = quantity;

                                prevSerial = serial;
                            }
                            else
                            {
                                // 来到这里的一定就是套餐内容
                                orderDetail.order_detail_setmeal.Add(new order_detail_setmeal()
                                {
                                    menu = new menu() { name = prodName }
                                });

                                if (!string.IsNullOrEmpty(remark2))
                                {
                                    string[] methods = remark2.Split(',');
                                    foreach (string method in methods)
                                        orderDetail.order_detail_setmeal.Last().order_detail_method.Add(
                                            new order_detail_method()
                                            {
                                                method = new method() { name = method }
                                            });
                                }
                            }
                        }

                        orderDetails.Add(orderDetail);
                    }
                    #endregion 填入orderDetails

                    PrintDialog printDlg = new PrintDialog();
                    var printers = new LocalPrintServer().GetPrintQueues();
                    var selectedPrinter = printers.FirstOrDefault(p => p.Name == Global.KitchenHuaDanPrinter);
                    if (selectedPrinter != null)
                    {
                        printDlg.PrintQueue = selectedPrinter;

                        PrintVisual.HuaDan huaDan = new PrintVisual.HuaDan()
                        {
                            TableNum = tableNum,
                            OrderNum = orderNo,
                            StaffName = waiter,
                        };

                        foreach (var orderDetail in orderDetails)
                            huaDan.AddItem(orderDetail);

                        huaDan.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                        huaDan.Arrange(new Rect(new Point(0, 0), huaDan.DesiredSize));
                        printDlg.PrintVisual(huaDan, "划单打印");
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }
#else
            try
            {
                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    #region Get orderDetails
                    if (prnType == 0)
                    {
                        orderDetails = context.order_detail
                            .Include("menu")
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            .Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            .Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }
                    else
                    {
                        var query = context.order_detail
                            .Where(n => n.order_id == orderId)
                            .OrderBy(n => n.group_cnt)
                            .GroupBy(n => n.group_cnt)
                            .ToList();

                        int key;
                        if (prnType == -1)
                            key = query.Last().Key;
                        else
                        {
                            if (prnType < query.Count)
                                key = query.ElementAt(prnType - 1).Key;
                            else
                                return "超出边界";
                        }

                        orderDetails = context.order_detail
                            .Include("menu")
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            .Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            .Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false && n.group_cnt == key)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }
                    #endregion Get orderDetails

                    #region 打印
                    // 打印
                    PrintDialog printDlg = new PrintDialog();
                    var printers = new LocalPrintServer().GetPrintQueues();
                    var selectedPrinter = printers.FirstOrDefault(p => p.Name == Global.KitchenHuaDanPrinter);
                    printDlg.PrintQueue = selectedPrinter;

                    PrintVisual.HuaDan huaDan = new PrintVisual.HuaDan() { 
                        TableNum = orderDetails[0].order.table_no,
                        StaffName = orderDetails[0].staff.name,
                        OrderNum = orderDetails[0].order.order_num,
                    };

                    string foxproOrderNum = GetOrderNumFromFoxproByTablenNum(orderDetails[0].order.table_no);
                    int serial = 0;
                    foreach (var orderDetail in orderDetails)
                    {
                        huaDan.AddItem(orderDetail);

                        #region 写入foxpro
                        _mutexFoxpro.WaitOne();
                        SaveOrder2Foxpro(orderDetail, foxproOrderNum, ++serial);
                        _mutexFoxpro.ReleaseMutex();
                        #endregion 写入foxpro
                    }

                    huaDan.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                    huaDan.Arrange(new Rect(new Point(0, 0), huaDan.DesiredSize));
                    printDlg.PrintVisual(huaDan, "划单打印");

                    #endregion 打印

                    Global.Logger.Debug(string.Format("({0}) - 划单打印", Global.TraceMessage()));
                }

            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }
#endif

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string PrintChuPin(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

#if FOXPRO
            try
            {
                string orderNo = GetFoxproOrderNo(orderId);
                if (string.IsNullOrEmpty(orderNo))
                {
                    Global.Logger.Debug(string.Format("PrintHuaDan中orderno出错:{0}", orderId));
                    return string.Format("PrintHuaDan中orderno出错:{0}", orderId);
                }

                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Empty;
                    string orderTime = null;
                    string tableNum = null;
                    string waiter = null;

                    #region 打印第几次，如果全打不执行
                    // 打印第几次，如果全打不执行
                    if (prnType != 0)
                    {
                        sql = string.Format("SELECT ordertime FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNo);
                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            OleDbDataReader reader = cmd.ExecuteReader();
                            int cnt = 1;
                            DateTime prevTime = DateTime.Now;
                            while (reader.Read())
                            {
                                DateTime ot = reader.GetDateTime(0);
                                if (prevTime != ot)
                                {
                                    orderTime = string.Format("{0}^{1}{2}", "{", ot.ToString("u"), "}");
                                    if (cnt == prnType)
                                        break;

                                    cnt++;

                                    prevTime = ot;
                                }
                            }
                        }
                    }
                    #endregion 打印第几次，如果全打不执行

                    if (orderTime == null)
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter, department, serialno FROM poh WHERE (orderno = '{0}') ORDER BY serial", orderNo);
                    else
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter, department, serialno FROM poh WHERE (orderno = '{0}') AND (ordertime = {1}) ORDER BY serial", orderNo, orderTime);

                    Dictionary<string, List<object>> prnGrp = new Dictionary<string, List<object>>();

                    #region 填入prnGrp
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string serial = reader.GetString(0).Trim();
                            string prodName = reader.GetString(1).Trim();
                            int quantity = reader.GetInt32(2);
                            string prnGrpKey = reader.GetString(3).Trim();
                            string remark1 = reader.GetString(4).Trim();
                            string remark2 = reader.GetString(5).Trim();
                            if (tableNum == null)
                                tableNum = reader.GetString(6).Trim();
                            if (waiter == null)
                                waiter = reader.GetString(7).Trim();
                            string department = reader.GetString(8).Trim();
                            string seriaoNo = reader.GetString(9).Trim();

                            if (!string.IsNullOrEmpty(prnGrpKey))
                            {
                                if (prnGrp.ContainsKey(prnGrpKey) == false)
                                    prnGrp.Add(prnGrpKey, new List<object>());

                                if (string.IsNullOrEmpty(remark1))
                                {
                                    // 这个不是套餐
                                    order_detail orderDetail = new order_detail()
                                    {
                                        quantity = 1,
                                        menu_id = Int32.Parse(seriaoNo),          // 在foxpro中，暂且用menu_id来记录serial
                                        menu = new menu() { id = Int32.Parse(department), name = prodName }
                                    };

                                    // 做法
                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            orderDetail.order_detail_method.Add(
                                                new order_detail_method()
                                                {
                                                    method = new method() { name = method }
                                                });
                                    }

                                    for (int i = 0; i < quantity; i++)
                                    {
                                        prnGrp[prnGrpKey].Add(orderDetail);
                                    }
                                }
                                else
                                {
                                    // 这个是套餐
                                    order_detail_setmeal setmeal = new order_detail_setmeal()
                                    {
                                        menu_id = Int32.Parse(seriaoNo),          // 在foxpro中，暂且用menu_id来记录serial
                                        menu = new menu() { id = Int32.Parse(department), name = prodName },
                                        order_detail = new order_detail() { quantity = 1, menu = new menu() { name = remark1 } },
                                    };

                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            setmeal.order_detail_method.Add(
                                                new order_detail_method()
                                                {
                                                    method = new method() { name = method }
                                                });
                                    }

                                    for (int i = 0; i < quantity; i++)
                                    {
                                        prnGrp[prnGrpKey].Add(setmeal);
                                    }
                                }
                            }
                        }
                    }
                    #endregion 填入prnGrp

                    PrintDialog printDlg = new PrintDialog();
                    #region 将数据填入visual
                    foreach (var key in prnGrp.Keys)
                    {
                        if (key == "NULL")
                            continue;

                        string prnName = GetFoxproPrinterNameByPG(key);
                        if (string.IsNullOrEmpty(prnName))
                            continue;

                        var printers = new LocalPrintServer().GetPrintQueues();
                        var selectedPrinter = printers.FirstOrDefault(p => p.Name == prnName);
                        if (selectedPrinter == null)
                            continue;

                        printDlg.PrintQueue = selectedPrinter;
                        Global.Logger.Debug(string.Format("printer name:{0}", prnName));

                        int currItem = 0;
                        int totalItem = prnGrp[key].Count();
                        foreach (var obj in prnGrp[key])
                        {
                            currItem++;

                            if (obj.GetType() == typeof(order_detail))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    
                                    cpd.OrderNum = orderNo;
                                    cpd.TableNum = tableNum;
                                    cpd.StaffName = waiter;
                                    cpd.OrderNum = orderNo;
                                    cpd.Department = ((order_detail)obj).menu.id == 11 ? "厨房" : "酒吧";
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.SerialNum = ((order_detail)obj).menu_id.ToString();
                                    cpd.AddItem((order_detail)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("({3}) - 共{0}张单的第{1}张 to {2}", totalItem, currItem, prnName, Global.TraceMessage()));
                                }
                            }
                            else if (obj.GetType() == typeof(order_detail_setmeal))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    cpd.OrderNum = orderNo;
                                    cpd.TableNum = tableNum;
                                    cpd.StaffName = waiter;
                                    cpd.OrderNum = orderNo;
                                    cpd.Department = ((order_detail_setmeal)obj).menu.id == 11 ? "厨房" : "酒吧";
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.SerialNum = ((order_detail_setmeal)obj).menu_id.ToString();
                                    cpd.AddItem((order_detail_setmeal)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("({3}) - 共{0}张单的第{1}张 to {2}", totalItem, currItem, prnName, Global.TraceMessage()));
                                }
                            }

                        }
                    }
                    #endregion 将数据填入visual


                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }
#else
            try
            {
                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    #region Get orderDetails
                    if (prnType == 0)
                    {
                        orderDetails = context.order_detail
                            .Include(n => n.menu)
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            .Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            .Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }
                    else
                    {
                        var query = context.order_detail
                            .Where(n => n.order_id == orderId)
                            .OrderBy(n => n.group_cnt)
                            .GroupBy(n => n.group_cnt)
                            .ToList();

                        int key;
                        if (prnType == -1)
                            key = query.Last().Key;
                        else
                        {
                            if (prnType < query.Count)
                                key = query.ElementAt(prnType - 1).Key;
                            else
                                return "超出边界";
                        }

                        orderDetails = context.order_detail
                            .Include("menu")
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            .Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            .Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false && n.group_cnt == key)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }
                #endregion Get orderDetails

                    #region 用来区分不同打印组下有哪些条目
                    // 用来区分不同打印组下有哪些条目, 以打印组的id为key
                    Dictionary<string, List<object>> prnGrp = new Dictionary<string, List<object>>();

                    foreach (order_detail orderDetail in orderDetails)
                    {
                        if ((orderDetail.order_detail_setmeal != null) &&
                            (orderDetail.order_detail_setmeal.Count() > 0))
                        {
                            foreach (order_detail_setmeal setmealItem in orderDetail.order_detail_setmeal)
                            {
                                if (prnGrp.ContainsKey(setmealItem.menu.printer_group_id == null ? "NULL" : setmealItem.menu.printer_group_id.ToString()) == false)
                                    prnGrp.Add(setmealItem.menu.printer_group_id == null ? "NULL" : setmealItem.menu.printer_group_id.ToString(), new List<object>());

                                // 如果数量大于1，每张单只代表一个量，因为这单要跟碟
                                for (int i = 0; i < setmealItem.order_detail.quantity; i++)
                                    prnGrp[setmealItem.menu.printer_group_id.ToString()].Add(setmealItem);
                            }
                        }
                        else
                        {
                            if (prnGrp.ContainsKey(orderDetail.menu.printer_group_id == null ? "NULL" : orderDetail.menu.printer_group_id.ToString()) == false)
                                prnGrp.Add(orderDetail.menu.printer_group_id == null ? "NULL" : orderDetail.menu.printer_group_id.ToString(), new List<object>());

                            // 如果数量大于1，每张单只代表一个量，因为这单要跟碟
                            for (int i = 0; i < orderDetail.quantity; i++)
                                prnGrp[orderDetail.menu.printer_group_id.ToString()].Add(orderDetail);
                        }
                    }
                    #endregion 用来区分不同打印组下有哪些条目

                    PrintDialog printDlg = new PrintDialog();
                    //string foxproOrderNum = GetOrderNumFromFoxproByTablenNum(orderDetails[0].order.table_no);
                    string foxproOrderNum = "111";
                    #region 将数据填入visual
                    foreach (var key in prnGrp.Keys)
                    {
                        if (key == "NULL")
                            continue;

                        printer pnt = GetPrinterNameByGroupId(Int32.Parse(key));
                        if (pnt == null)
                            continue;
                        if (string.IsNullOrEmpty(pnt.name))
                            continue;
                        string pgName = pnt.printer_group.name;

                        var printers = new LocalPrintServer().GetPrintQueues();
                        var selectedPrinter = printers.FirstOrDefault(p => p.Name == pnt.name);
                        printDlg.PrintQueue = selectedPrinter;
                        Global.Logger.Debug(string.Format("printer name:{0}", pnt.name));

                        int? prnCnt = GetAndAddPrintCnt(pnt.id);

                        int currItem = 0;
                        int totalItem = prnGrp[key].Count();
                        foreach (var obj in prnGrp[key])
                        {
                            currItem++;

                            if (obj.GetType().BaseType == typeof(order_detail))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    cpd.OrderNum = foxproOrderNum;
                                    cpd.Department = pgName;
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.SerialNum = prnCnt.ToString();
                                    cpd.AddItem((order_detail)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("({3}) - 共{0}张单的第{1}张 to {2}", totalItem, currItem, pnt.name, Global.TraceMessage()));
                                }
                            }
                            else if (obj.GetType().BaseType == typeof(order_detail_setmeal))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    cpd.OrderNum = foxproOrderNum;
                                    cpd.Department = pgName;
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.SerialNum = prnCnt.ToString();
                                    cpd.AddItem((order_detail_setmeal)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("({3}) - 共{0}张单的第{1}张 to {2}", totalItem, currItem, pnt.name, Global.TraceMessage()));
                                }
                            }

                        }
                    }
                    #endregion 将数据填入visual
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }
#endif

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string PrintLiuTai(int orderId, int prnType)
        {
            Global.Logger.Trace(Global.TraceMessage());

#if FOXPRO
            try
            {
                string orderNo = GetFoxproOrderNo(orderId);
                if (string.IsNullOrEmpty(orderNo))
                {
                    Global.Logger.Debug(string.Format("PrintHuaDan中orderno出错:{0}", orderId));
                    return string.Format("PrintHuaDan中orderno出错:{0}", orderId);
                }

                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Empty;
                    string orderTime = null;
                    string tableNum = null;
                    string waiter = null;
                    int totalOrderCnt = 0;

                    #region 打印第几次，如果全打不执行
                    // 打印第几次，如果全打不执行
                    if (prnType != 0)
                    {
                        sql = string.Format("SELECT ordertime FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNo);
                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            OleDbDataReader reader = cmd.ExecuteReader();
                            int cnt = 1;
                            DateTime prevTime = DateTime.Now;
                            while (reader.Read())
                            {
                                DateTime ot = reader.GetDateTime(0);
                                if (prevTime != ot)
                                {
                                    orderTime = string.Format("{0}^{1}{2}", "{", ot.ToString("u"), "}");
                                    if (cnt == prnType)
                                        break;

                                    cnt++;

                                    prevTime = ot;
                                }
                            }
                        }
                    }
                    #endregion 打印第几次，如果全打不执行

                    #region 看点了几次菜
                    sql = string.Format("SELECT ordertime FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNo);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        DateTime prevTime = DateTime.Now;
                        while (reader.Read())
                        {
                            DateTime ot = reader.GetDateTime(0);
                            if (prevTime != ot)
                            {
                                totalOrderCnt++;
                                prevTime = ot;
                            }
                        }
                    }
                    #endregion 看点亮几次菜

                    if (orderTime == null)
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter, department FROM poh WHERE (orderno = '{0}') ORDER BY serial", orderNo);
                    else
                        sql = string.Format("SELECT serial, prodname, quantity, printgroup, remark1, remark2, tableno, waiter, department FROM poh WHERE (orderno = '{0}') AND (ordertime = {1}) ORDER BY serial", orderNo, orderTime);

                    List<order_detail> orderDetails = new List<order_detail>();
                    #region 填入orderDetails
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        string prevSerial = string.Empty;
                        order_detail orderDetail = null;
                        order_detail_setmeal setmeal = null;
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string serial = reader.GetString(0).Trim();
                            string prodName = reader.GetString(1).Trim();
                            int quantity = reader.GetInt32(2);
                            string prnGrpKey = reader.GetString(3).Trim();
                            string remark1 = reader.GetString(4).Trim();
                            string remark2 = reader.GetString(5).Trim();
                            if (tableNum == null)
                                tableNum = reader.GetString(6).Trim();
                            if (waiter == null)
                                waiter = reader.GetString(7).Trim();
                            string department = reader.GetString(8).Trim();

                            if (prevSerial != serial)
                            {
                                if (orderDetail != null)
                                    orderDetails.Add(orderDetail);

                                orderDetail = new order_detail() { quantity = (decimal)quantity };

                                // 看是否套餐
                                if (string.IsNullOrEmpty(remark1))
                                {
                                    // 不是套餐
                                    orderDetail.price = GetFoxproPrice(orderNo, prodName);
                                    orderDetail.menu = new menu() { name = prodName };

                                    //// 做法
                                    //if (!string.IsNullOrEmpty(remark2))
                                    //{
                                    //    string[] methods = remark2.Split(',');
                                    //    foreach (string method in methods)
                                    //        orderDetail.order_detail_method.Add(
                                    //            new order_detail_method()
                                    //            {
                                    //                method = new method() { name = method }
                                    //            });
                                    //}
                                }
                                else
                                {
                                    // 是套餐
                                    orderDetail.price = GetFoxproPrice(orderNo, remark1);
                                    orderDetail.menu = new menu() { name = remark1 };
                                    setmeal = new order_detail_setmeal()
                                    {
                                        menu = new menu() { name = prodName }
                                    };

                                    //// 做法
                                    //if (!string.IsNullOrEmpty(remark2))
                                    //{
                                    //    string[] methods = remark2.Split(',');
                                    //    foreach (string method in methods)
                                    //        setmeal.order_detail_method.Add(
                                    //            new order_detail_method()
                                    //            {
                                    //                method = new method() { name = method }
                                    //            });
                                    //}

                                    orderDetail.order_detail_setmeal.Add(setmeal);
                                }

                                prevSerial = serial;
                            }
                            else
                            {
                                // 来到这里的肯定是套餐内容
                                setmeal = new order_detail_setmeal()
                                {
                                    menu = new menu() { name = prodName }
                                };

                                //// 做法
                                //if (!string.IsNullOrEmpty(remark2))
                                //{
                                //    string[] methods = remark2.Split(',');
                                //    foreach (string method in methods)
                                //        setmeal.order_detail_method.Add(
                                //            new order_detail_method()
                                //            {
                                //                method = new method() { name = method }
                                //            });
                                //}

                                orderDetail.order_detail_setmeal.Add(setmeal);
                            }
                        }

                        orderDetails.Add(orderDetail);
                    }
                    #endregion 填入orderDetails

                    PrintDialog printDlg = new PrintDialog();
                    var printers = new LocalPrintServer().GetPrintQueues();
                    var selectedPrinter = printers.FirstOrDefault(p => p.Name == GetPrinterNameByTableNum(tableNum));
                    if (selectedPrinter != null)
                    {
                        printDlg.PrintQueue = selectedPrinter;

                        PrintVisual.LiuTaiDan liuTai = new PrintVisual.LiuTaiDan()
                        {
                            OrderCount = totalOrderCnt.ToString(),
                            TableNum = tableNum,
                            StaffName = waiter,
                            OrderNum = orderNo,
                            TotalPrice = GetFoxproTotalPrice(orderNo),
                        };

                        foreach (var orderDetail in orderDetails)
                        {
                            liuTai.AddItem(orderDetail);
                        }

                        liuTai.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                        liuTai.Arrange(new Rect(new Point(0, 0), liuTai.DesiredSize));
                        printDlg.PrintVisual(liuTai, "留台单打印");

                        Global.Logger.Debug(string.Format("留台单打印到: {0}, {1}", GetPrinterNameByTableNum(tableNum), Global.TraceMessage()));
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(string.Format("LiuTai:{0}", ex.Message));
            }
#else
            try
            {
                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    #region Get orderDetails
                    if (prnType == 0)
                    {
                        orderDetails = context.order_detail
                            .Include(n => n.menu)
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            //.Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            //.Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false)
                            .OrderBy(n => n.order_time)
                            .ToList();

                        orderCount = "所有";
                    }
                    else
                    {
                        var query = context.order_detail
                            .Where(n => n.order_id == orderId)
                            .OrderBy(n => n.group_cnt)
                            .GroupBy(n => n.group_cnt)
                            .ToList();

                        int key;
                        if (prnType == -1)
                        {
                            key = query.Last().Key;
                            orderCount = query.Count.ToString();
                        }
                        else
                        {
                            if (prnType < query.Count)
                                key = query.ElementAt(prnType - 1).Key;
                            else
                                return "超出边界";

                            orderCount = prnType.ToString();
                        }

                        orderDetails = context.order_detail
                            .Include("menu")
                            .Include(n => n.order_detail_method.Select(m => m.method))
                            //.Include(n => n.order_detail_setmeal.Select(m => m.menu))
                            //.Include(n => n.order_detail_setmeal.Select(m => m.order_detail_method.Select(o => o.method)))
                            .Include("staff")
                            .Where(n => n.order_id == orderId && n.is_cancle == false && n.group_cnt == key)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }
                    #endregion Get orderDetails

                    PrintDialog printDlg = new PrintDialog();
                    var printers = new LocalPrintServer().GetPrintQueues();
                    var selectedPrinter = printers.FirstOrDefault(p => p.Name == GetPrinterNameByTableNum(orderDetails[0].order.table_no));
                    printDlg.PrintQueue = selectedPrinter;

                    PrintVisual.LiuTaiDan liuTai = new PrintVisual.LiuTaiDan()
                    {
                        OrderCount = orderCount,
                        TableNum = orderDetails[0].order.table_no,
                        StaffName = orderDetails[0].staff.name,
                        OrderNum = GetOrderNumFromFoxproByTablenNum(orderDetails[0].order.table_no),
                        TotalPrice = (decimal)orderDetails[0].order.receivable,
                    };

                    foreach (var orderDetail in orderDetails)
                    {
                        liuTai.AddItem(orderDetail);
                    }

                    liuTai.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                    liuTai.Arrange(new Rect(new Point(0, 0), liuTai.DesiredSize));
                    printDlg.PrintVisual(liuTai, "留台单打印");

                    Global.Logger.Debug(string.Format("({0}) - 留台单打印", Global.TraceMessage()));
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }
#endif

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string GetPrinterNameByTableNum(string tableNum)
        {
            string rtn = string.Empty;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql = string.Format("SELECT prntr FROM prntrb WHERE printgroup = '{0}'", tableNum);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    rtn = (string)cmd.ExecuteScalar();
                }

                conn.Close();
            }

            return rtn;
        }

        /// <summary>
        /// 根据groupid返回打印机名，如果一个group下有多个打印机，考虑返回最合适的打印机
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        printer GetPrinterNameByGroupId(int groupId)
        {
            using (var context = new gcafeEntities())
            {
                printer_group pg = context.printer_group.Include("printer").Where(n => n.id == groupId && n.branch_id == Global.BranchId).FirstOrDefault();

                if (pg != null && pg.printer != null && pg.printer.Count() > 0)
                    return pg.printer.FirstOrDefault();
                else
                    return (printer)null;
            }
        }

        string GetPrinterGroupNameByGroupId(int groupId)
        {
            using (var context = new gcafeEntities())
            {
                printer_group pg = context.printer_group.Where(n => n.id == groupId && n.branch_id == Global.BranchId).FirstOrDefault();

                if (pg != null)
                    return pg.name;
                else
                    return string.Empty;
            }
        }

        int? GetAndAddPrintCnt(int printerId, bool isInc = true)
        {
            int? rtn = 0;
            using (var context = new gcafeEntities())
            {
                _mutex.WaitOne();
                rtn = context.IncreaseAndResetPrintCnt(printerId, isInc).FirstOrDefault();
                _mutex.ReleaseMutex();

                return rtn;
            }
        }

        public void AddTask(PrintTask printTask)
        {
            _mutex.WaitOne();
            Enqueue(printTask);
            _mutex.ReleaseMutex();
        }

        string GetOrderNumFromFoxproByTablenNum(string tableNum)
        {
            string rtn = string.Empty;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql = string.Format("SELECT orderno FROM orders WHERE tableno = '{0}' AND paid = 0", tableNum);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    rtn = (string)cmd.ExecuteScalar();
                    if (rtn == null)
                        rtn = string.Empty;
                }

                conn.Close();
            }

            return rtn;
        }

        string SaveOrder2Foxpro(order_detail orderDetail, string orderNum, int serial)
        {
            string rtn = string.Empty;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql;

                if (orderNum != null && orderNum.Length > 0)
                {
                    sql = string.Format("SELECT price2, productnn FROM product WHERE productno = '{0}'", orderDetail.menu.number);
                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        decimal price2 = reader.GetDecimal(0);
                        string prodNn = reader.GetString(1);

                        sql = string.Format("INSERT INTO orditem(ordertime, orderno, productno, prodname, price, price2, quantity, note1name, note2name, note1no, price1, quantity1, add10, quantity2, discount, machineid, taiji, memberno, amt, productnn, note2no) VALUES({0}, '{1}', '{2}', '{3}', {4}, {5}, {6}, '11', '', '', 0, 0, 0, 0, 1, '{7}', 0, '', 0, '{8}', '')",
                            "{ fn NOW() }",
                            orderNum,
                            orderDetail.menu.number,
                            orderDetail.menu.name,
                            orderDetail.price,
                            price2,
                            orderDetail.quantity,
                            "A",
                            prodNn);

                        using (var cmd1 = new OleDbCommand(sql, conn))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        sql = string.Format("SELECT orderno FROM orditem WHERE orderno = '{0}'", orderNum);
                        using (var cmd1 = new OleDbCommand(sql, conn))
                        {
                            OleDbDataReader r = cmd1.ExecuteReader();
                            if (!r.Read())
                                throw new Exception("error");
                        }

                        if (orderDetail.order_detail_setmeal.Count > 0)
                        {
                            // 这是套餐
                            foreach (var setmeal in orderDetail.order_detail_setmeal)
                            {
                                sql = string.Format("INSERT INTO poh(department, ordertime, orderno, serialno, prodname, machineid, quantity, tableno, itemno, printgroup, remark1, remark2, waiter, serial) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13:D2}')",
                                     setmeal.menu.number.Substring(0, 2), "{ fn NOW() }", orderNum, GenSerialNo(setmeal.menu.number), setmeal.menu.name, setmeal.order_detail.device_id, setmeal.order_detail.quantity, setmeal.order_detail.order.table_no, "0", GetPrintGroup(setmeal.menu.number), setmeal.order_detail.menu.name, GetZuoFa(setmeal.order_detail_method), setmeal.order_detail.order.staff2.number, serial);

                                using (var cmd1 = new OleDbCommand(sql, conn))
                                {
                                    cmd1.ExecuteNonQuery();
                                }

                            }
                        }
                        else
                        {
                            // 不是套餐
                            sql = string.Format("INSERT INTO poh(department, ordertime, orderno, serialno, prodname, machineid, quantity, tableno, itemno, printgroup, remark1, remark2, waiter, serial) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13:D2}')",
                                orderDetail.menu.number.Substring(0, 2), "{ fn NOW() }", orderNum, GenSerialNo(orderDetail.menu.number), orderDetail.menu.name, orderDetail.device_id, orderDetail.quantity, orderDetail.order.table_no, "0", GetPrintGroup(orderDetail.menu.number), "", GetZuoFa(orderDetail.order_detail_method), orderDetail.order.staff2.number, serial);

                            using (var cmd1 = new OleDbCommand(sql, conn))
                            {
                                cmd1.ExecuteNonQuery();
                            }
                        }

                        sql = string.Format("SELECT orderno FROM poh WHERE orderno = '{0}'", orderNum);
                        using (var cmd1 = new OleDbCommand(sql, conn))
                        {
                            OleDbDataReader r = cmd1.ExecuteReader();
                            if (!r.Read())
                                throw new Exception("error");
                        }

                    }

                    conn.Close();
                }
            }

            return rtn;
        }

        /// <summary>
        /// 取回打印组
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        string GetPrintGroup(string productNo)
        {
            string pg = string.Empty;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    using (var cmd = new OleDbCommand(string.Format("SELECT printgroup FROM product WHERE productno = '{0}'", productNo), conn))
                    {
                        pg = cmd.ExecuteScalar() as string;
                        if (pg != null)
                            pg = pg.Trim();
                        else
                            pg = string.Empty;
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
            }

            return pg;
        }

        /// <summary>
        /// 生成poh.serialno
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        private int GenSerialNo(string productNo)
        {
            int serialNo = -1;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql;

                    if (productNo.Substring(0, 2) == "11")
                        sql = "SELECT kitchen FROM lastsn";
                    else if (productNo.Substring(0, 2) == "22")
                        sql = "SELECT bar FROM lastsn";
                    else
                        return -1;

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        serialNo = Int32.Parse(cmd.ExecuteScalar() as string);
                    }

                    if (productNo.Substring(0, 2) == "11")
                        sql = string.Format("UPDATE lastsn SET kitchen = '{0}'", serialNo + 1);
                    else if (productNo.Substring(0, 2) == "22")
                        sql = string.Format("UPDATE lastsn SET bar = '{0}'", serialNo + 1);

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message);
                return -1;
            }

            return serialNo;
        }

        string GetZuoFa(ICollection<order_detail_method> methods)
        {
            string zuofa = string.Empty;

            if (methods != null && methods.Count > 0)
            {
                foreach (var method in methods)
                {
                    if (string.IsNullOrEmpty(zuofa))
                        zuofa = method.method.name;
                    else
                        zuofa += "，" + method.method.name;
                }
            }

            return zuofa;
        }

        string GetFoxproPrinterNameByPG(string pg)
        {
            string rtn = string.Empty;

            using (var conn = new OleDbConnection(Global.FoxproPrntrPath))
            {
                conn.Open();

                string sql = string.Format("SELECT prntr FROM prntr WHERE printgroup = '{0}'", pg);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    rtn = (string)cmd.ExecuteScalar();
                    if (rtn != null)
                        rtn = rtn.Trim();
                }

                conn.Close();
            }

            //return "PDFCreator";
            return rtn;
        }

        string GetFoxproOrderNo(int orderId)
        {
            string orderNo = string.Empty;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql = "SELECT fax FROM sysinfo";
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    string fax = (string)cmd.ExecuteScalar();
                    if (fax != null)
                    {
                        orderNo = fax.Trim() + orderId.ToString();
                    }
                }

                conn.Close();
            }

            return orderNo;
        }

        decimal GetFoxproTotalPrice(string orderNum)
        {
            decimal rtn = 0;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql = string.Format("SELECT price FROM orditem WHERE (orderno = '{0}')", orderNum);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rtn += reader.GetDecimal(0);
                    }
                }

                conn.Close();
            }

            return rtn;
        }

        decimal GetFoxproPrice(string orderNum, string prodName)
        {
            decimal rtn = 0;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string sql = string.Format("SELECT price FROM orditem WHERE (orderno = '{0}') AND (prodname = '{1}')", orderNum, prodName);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    object val = cmd.ExecuteScalar();
                    if (val != null)
                        rtn = (decimal)val;
                }

                conn.Close();
            }

            return rtn;
        }

        bool IsNeedToTidy()
        {
            bool rtn = false;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT ");
                    using (var cmd = new OleDbCommand(sql, conn))
                    {

                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return rtn;
        }

        bool IsNeed10Percent()
        {
            bool rtn = false;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT ");
                    using (var cmd = new OleDbCommand(sql, conn))
                    {

                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return rtn;
        }

        string GetFoxproProductNumByName(string prodName)
        {
            string rtn = string.Empty;

            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT productno FROM product WHERE prodname = '{0}'", prodName);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        rtn = (string)cmd.ExecuteScalar();
                        if (rtn == null)
                            rtn = string.Empty;
                        else
                            rtn = rtn.Trim();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return rtn;
        }

        void FoxproTidyup()
        {
            try
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT ordertime, orderno, serial, prodname, printgroup FROM poh ORDER BY ordertime");
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        DateTime orderTimePrev;
                        string orderNoPrev = string.Empty;
                        string prodNamePrev = string.Empty;
                        string printGroupPrev = string.Empty;
                        string remark = string.Empty;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DateTime orderTime = reader.GetDateTime(0);
                            string orderNo = reader.GetString(1);
                            string serial = reader.GetString(2);
                            string prodName = reader.GetString(3);
                            string printGroup = reader.GetString(4);
                            string productNo = GetFoxproProductNumByName(prodName);

                            if ((productNo == "111901" || productNo.Substring(0, 2) == "33") &&     // 111901是加铁板
                                printGroup == printGroupPrev)
                            {
                                if (string.IsNullOrEmpty(remark))
                                    remark = prodName;
                                else
                                    remark += "," + prodName;
                            }
                            else
                            {
                                orderTimePrev = orderTime;
                                orderNoPrev = orderNo;
                                prodNamePrev = prodName;
                                printGroupPrev = printGroup;
                                remark = string.Empty;
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Dispose()
        {
            _isStop = true;
            //Task.WaitAny(new Task[] { _taskListScanner });
            //tokenSource.Cancel();

            //if (!_taskListScanner.IsCanceled && !_taskListScanner.IsFaulted)
            //{
            //    //_taskListScanner.Dispose();
            //    System.Diagnostics.Debug.WriteLine("task canceled ok");
            //}
            //else
            //    System.Diagnostics.Debug.WriteLine("can't cancel task");
        }
    }
}
