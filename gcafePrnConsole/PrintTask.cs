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

                    await Print(printTask);
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
                            .Where(n => n.order_id == 1 && n.is_cancle == false && n.group_cnt == query.Last().Key)
                            .OrderBy(n => n.order_time)
                            .ToList();
                    }

                    #region 用了区分不同打印组下有哪些条目
                    // 用了区分不同打印组下有哪些条目, 以打印组的id为key
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
                    #endregion 用了区分不同打印组下有哪些条目

                    PrintDialog printDlg = new PrintDialog();

                    #region 将数据填入visual
                    gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan();
                    foreach (var key in prnGrp.Keys)
                    {
                        string printerName = await GetPrinterNameByGroupId(Int32.Parse(key));

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
                    #endregion 将数据填入visual


                }
            }
            catch (Exception ex)
            {

            }

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
        async Task<string> GetPrinterNameByGroupId(int groupId)
        {
            using (var context = new gcafeEntities())
            {
                return "";
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
