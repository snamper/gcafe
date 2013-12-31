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

namespace gcafePrnConsole
{
    public class PrintTask
    {
        public enum PrintType { PrintLiuTai, PrintChuPin, PringHuaDan };

        PrintType _type;
        int _orderId;
        int _prnType;
        int _orderDetailId;
        int _setmealId;

        public PrintTask(PrintType type, int orderId, int prnType, int orderDetailId = -1, int setmealId = -1)
        {
            _type = type;
            _prnType = prnType;
            _orderId = orderId;
            _orderDetailId = orderDetailId;
            _setmealId = setmealId;
        }
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
                PrintDialog printDlg = new PrintDialog();
                printDlg.ShowDialog();
                //var printers = new LocalPrintServer().GetPrintQueues();
                //var selectedPrinter = printers.FirstOrDefault(p => p.Name == "Microsoft XPS Document Writer");
                //printDlg.PrintQueue = selectedPrinter;

                List<order_detail> orderDetails;
                using (var context = new gcafeEntities())
                {
                    var query = context.order_detail.Include("menu").Include("order_detail_method.method").Include("order_detail_setmeal.menu").Include("staff").Where(n => n.order_id == 1).GroupBy(n => (System.DateTime.Now - n.order_time).TotalSeconds);
                    foreach (var vg in query)
                    {
                        Console.WriteLine("Age group: {0}  Number of pets: {1}", vg.Key, vg.Count());
                    }
                }


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
