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
    /// Interaction logic for ChuPinDan.xaml
    /// </summary>
    public partial class ChuPinDan : UserControl
    {
        public ChuPinDan()
        {
            InitializeComponent();
        }

        public string Department
        {
            set
            {
                Depart.Text = value;
            }
        }

        public string TableNum
        {
            set
            {
                TableNo.Text = value;
            }
        }

        public string StaffName
        {
            set
            {
                tbWaiter.Text = value;
            }
        }

        public string SerialNum
        {
            set
            {
                _serialNum.Text = value;
            }
        }

        public string PageCnt
        {
            set
            {
                _pageCnt.Text = value;
            }
        }

        public string OrderNum
        {
            set
            {
                _orderNum.Text = value;
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
