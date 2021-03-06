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

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string PrintChuPin(int orderId, int prnType, bool rePrint = false)
        {
            Global.Logger.Trace(Global.TraceMessage());

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
                                if (prnGrp.ContainsKey(setmealItem.menu.printer_group_id.ToString()) == false)
                                    prnGrp.Add(setmealItem.menu.printer_group_id.ToString(), new List<object>());

                                // 如果数量大于1，每张单只代表一个量，因为这单要跟碟
                                for (int i = 0; i < setmealItem.order_detail.quantity; i++)
                                    prnGrp[setmealItem.menu.printer_group_id.ToString()].Add(setmealItem);
                            }
                        }
                        else
                        {
                            if (prnGrp.ContainsKey(orderDetail.menu.printer_group_id.ToString()) == false)
                                prnGrp.Add(orderDetail.menu.printer_group_id.ToString(), new List<object>());

                            // 如果数量大于1，每张单只代表一个量，因为这单要跟碟
                            for (int i = 0; i < orderDetail.quantity; i++)
                                prnGrp[orderDetail.menu.printer_group_id.ToString()].Add(orderDetail);
                        }
                    }
                    #endregion 用来区分不同打印组下有哪些条目

                    PrintDialog printDlg = new PrintDialog();
                    string foxproOrderNum = GetOrderNumFromFoxproByTablenNum(orderDetails[0].order.table_no);

                    #region 将数据填入visual
                    foreach (var key in prnGrp.Keys)
                    {
                        printer pnt = GetPrinterNameByGroupId(Int32.Parse(key));
                        string pgName = pnt.printer_group.name;

                        var printers = new LocalPrintServer().GetPrintQueues();
                        var selectedPrinter = printers.FirstOrDefault(p => p.Name == pnt.name);
                        printDlg.PrintQueue = selectedPrinter;

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

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string PrintLiuTai(int orderId, int prnType)
        {
            Global.Logger.Trace(Global.TraceMessage());

            string orderCount = string.Empty;

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

            Global.Logger.Trace(Global.TraceMessage());

            return "";
        }

        string GetPrinterNameByTableNum(string tableNum)
        {
            return "留台4";
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
