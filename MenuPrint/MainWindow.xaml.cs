using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;

namespace MenuPrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //VisualPrint.VisualPrintDialog dlg = new VisualPrint.VisualPrintDialog(new MenuItem() { WPWidth = 800 });
            //dlg.ShowDialog("HP LaserJet 6L");

            PrintDialog printDlg = new PrintDialog();
            var printers = new LocalPrintServer().GetPrintQueues();
            var selectedPrinter = printers.FirstOrDefault(p => p.Name == "PDFCreator");
            printDlg.PrintQueue = selectedPrinter;
            printDlg.PrintVisual(new MenuItem() { WPWidth = 800 }, "二维码打印");
        }
    }
}
