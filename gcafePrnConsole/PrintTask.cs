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
        public enum PrintType { PrintLiuTai, PrintChuPin, PringHuaDan };

        public PrintTask(PrintType type, int orderId, int prnType, int orderDetailId = -1, int setmealId = -1)
        {
            Type = type;
            PrnType = prnType;
            OrderId = orderId;
            OrderDetailId = orderDetailId;
            SetmealId = setmealId;
        }

        public PrintType Type { get; private set; }
        public int OrderId { get; private set; }
        public int PrnType { get; private set; }
        public int OrderDetailId { get; private set; }
        public int SetmealId { get; private set; }
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

                    await Print(printTask);
                }

                Thread.Sleep(1000);
            }
        }

        private delegate Task<string> DoTask(PrintTask printTask);

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

                PrintDialog printDlg = new PrintDialog();
                //printDlg.ShowDialog();

                var printers = new LocalPrintServer().GetPrintQueues();
                var selectedPrinter = printers.FirstOrDefault(p => p.Name == "PDFCreator");
                printDlg.PrintQueue = selectedPrinter;


                gcafePrnConsole.PrintVisual.ChuPinDan cpd = new PrintVisual.ChuPinDan();
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
