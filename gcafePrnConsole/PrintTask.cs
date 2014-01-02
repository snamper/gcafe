﻿using System;
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


namespace gcafePrnConsole
{
    public class PrintTask
    {
        public enum PrintType { PrintLiuTai, PrintChuPin, PrintHuaDan };

        public PrintTask(PrintType type, int orderId, int prnType, int orderDetailId = -1, int setmealId = -1)
        {
            Type = type;
            PrnType = prnType;
            OrderId = orderId;
            OrderDetailId = orderDetailId;
            SetmealId = setmealId;
            IsUrge = !(OrderDetailId == -1 && SetmealId == -1);
        }

        public PrintType Type { get; private set; }
        public int OrderId { get; private set; }
        public int PrnType { get; private set; }
        public int OrderDetailId { get; private set; }
        public int SetmealId { get; private set; }
        public bool IsUrge { get; set; }
    }

    public class PrintTaskMgr : Queue<PrintTask>, IDisposable
    {
        Thread _thrTask;
        bool _isStop = false;
        Task _taskListScanner;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Mutex _mutex = new Mutex();

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

        async void ThreadTask()
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
                            await PrintHuaDan(printTask.OrderId, printTask.PrnType);
                            break;

                        case PrintTask.PrintType.PrintChuPin:
                            await PrintChuPin(printTask.OrderId, printTask.PrnType);
                            break;

                        case PrintTask.PrintType.PrintLiuTai:
                            await PrintLiuTai(printTask.OrderId, printTask.PrnType);
                            break;
                    }

                    //await Print(printTask);
                }

                Thread.Sleep(1000);
            }
        }

        async Task<string> PrintHuaDan(int orderId, int prnType)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }

            return "";
        }

        async Task<string> PrintChuPin(int orderId, int prnType)
        {
            Global.Logger.Trace(Global.TraceMessage());

            try
            {
                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    if (prnType == 0)
                    {
                        orderDetails = context.order_detail
                            .Include("menu")
                            .Include("order_detail_method.method")
                            .Include("order_detail_setmeal.menu")
                            .Include("order_detail_setmeal.order_detail_method.method")
                            .Include("staff")
                            .Where(n => n.order_id == 1 && n.is_cancle == false)
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
                            .Include("order_detail_method.method")
                            .Include("order_detail_setmeal.menu")
                            .Include("order_detail_setmeal.order_detail_method.method")
                            .Include("staff")
                            .Where(n => n.order_id == 1 && n.is_cancle == false && n.group_cnt == key)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }

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

                    #region 将数据填入visual
                    foreach (var key in prnGrp.Keys)
                    {
                        printer pnt = await GetPrinterNameByGroupId(Int32.Parse(key));
                        string pgName = pnt.printer_group.name;

                        var printers = new LocalPrintServer().GetPrintQueues();
                        var selectedPrinter = printers.FirstOrDefault(p => p.Name == pnt.name);
                        printDlg.PrintQueue = selectedPrinter;

                        int? prnCnt = await GetAndAddPrintCnt(pnt.id);

                        int currItem = 0;
                        int totalItem = prnGrp[key].Count();
                        foreach (var obj in prnGrp[key])
                        {
                            currItem++;

                            if (obj.GetType().BaseType == typeof(order_detail))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    cpd.Department = pgName;
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.AddItem((order_detail)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("共{0}张单的第{1}张 to {2}", totalItem, currItem, pnt.name));
                                }
                            }
                            else if (obj.GetType().BaseType == typeof(order_detail_setmeal))
                            {
                                using (gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan())
                                {
                                    cpd.Department = pgName;
                                    cpd.PageCnt = string.Format("共{0}张单的第{1}张", totalItem, currItem);
                                    cpd.AddItem((order_detail_setmeal)obj);
                                    cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                                    cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                                    printDlg.PrintVisual(cpd, "出品单打印");

                                    Global.Logger.Debug(string.Format("共{0}张单的第{1}张 to {2}", totalItem, currItem, pnt.name));
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

        async Task<string> PrintLiuTai(int orderId, int prnType)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }

            return "";
        }

        /// <summary>
        /// 根据groupid返回打印机名，如果一个group下有多个打印机，考虑返回最合适的打印机
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        async Task<printer> GetPrinterNameByGroupId(int groupId)
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

        async Task<string> GetPrinterGroupNameByGroupId(int groupId)
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

        async Task<int?> GetAndAddPrintCnt(int printerId, bool isInc = true)
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


        public async Task<string> Print(PrintTask printTask)
        {
            try
            {
                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    var query = context.order_detail.Where(n => n.order_id == printTask.OrderId).OrderBy(n => n.group_cnt).GroupBy(n => n.group_cnt).ToList();

                    int key = query.Last().Key;
                    orderDetails = context.order_detail.Include("menu").Include("order_detail_method.method").Include("order_detail_setmeal.menu").Include("order_detail_setmeal.order_detail_method.method").Include("staff").Where(n => n.order_id == 1 && n.group_cnt == key).ToList();
                }

                Dictionary<string, List<object>> prnGrp = new Dictionary<string, List<object>>();

                PrintDialog printDlg = new PrintDialog();
                //printDlg.ShowDialog();

                gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan();

                foreach (order_detail orderDetail in orderDetails)
                {
                    if ((orderDetail.order_detail_setmeal != null) &&
                        (orderDetail.order_detail_setmeal.Count() > 0))
                    {
                        foreach (order_detail_setmeal setmealItem in orderDetail.order_detail_setmeal)
                        {
                            if (prnGrp.ContainsKey(setmealItem.menu.printer_group_id.ToString()) == false)
                                prnGrp.Add(setmealItem.menu.printer_group_id.ToString(), new List<object>());

                            prnGrp[setmealItem.menu.printer_group_id.ToString()].Add(setmealItem);
                        }
                    }
                    else
                    {
                        if (prnGrp.ContainsKey(orderDetail.menu.printer_group_id.ToString()) == false)
                            prnGrp.Add(orderDetail.menu.printer_group_id.ToString(), new List<object>());

                        prnGrp[orderDetail.menu.printer_group_id.ToString()].Add(orderDetail);
                    }
                }

                foreach (var key in prnGrp.Keys)
                {
                    int currItem = 0;
                    int totalItem = prnGrp[key].Count();
                    foreach (var obj in prnGrp[key])
                    {
                        if (obj.GetType().BaseType == typeof(order_detail))
                        {
                            cpd.AddItem((order_detail)obj);
                        }
                        else if (obj.GetType().BaseType == typeof(order_detail_setmeal))
                        {
                            cpd.AddItem((order_detail_setmeal)obj);
                        }

                    }
                }

                

                var printers = new LocalPrintServer().GetPrintQueues();
                var selectedPrinter = printers.FirstOrDefault(p => p.Name == "PDFCreator");
                printDlg.PrintQueue = selectedPrinter;


                
                cpd.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                cpd.Arrange(new Rect(new Point(0, 0), cpd.DesiredSize));
                printDlg.PrintVisual(new gcafePrnConsole.PrintVisual.ChuPinDan(), "test");
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return "";
        }

        public void AddTask(PrintTask printTask)
        {
            _mutex.WaitOne();
            Enqueue(printTask);
            _mutex.ReleaseMutex();

            System.Diagnostics.Debug.WriteLine("========================================= p");
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