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

namespace gcafePrnConsole.PrintVisual
{
    /// <summary>
    /// Interaction logic for HuaDan.xaml
    /// </summary>
    public partial class HuaDan : UserControl
    {
        public HuaDan()
        {
            InitializeComponent();

            Time = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }

        public string Department
        {
            set
            {
                tbDepart.Text = value;
            }
        }

        public string TableNum
        {
            set
            {
                tbTableNum.Text = value;
            }
        }

        public string SerialNum
        {
            set
            {
                tbSerialNum.Text = value;
            }
        }

        public string StaffName
        {
            set
            {
                tbWaiter.Text = value;
            }
        }

        public string Time
        {
            set
            {
                tbTime.Text = value;
            }
        }

    }
}
