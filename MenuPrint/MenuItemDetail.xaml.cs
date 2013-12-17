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

namespace MenuPrint
{
    /// <summary>
    /// Interaction logic for MenuItemDetail.xaml
    /// </summary>
    public partial class MenuItemDetail : UserControl
    {
        public MenuItemDetail()
        {
            InitializeComponent();
        }

        public string Code
        {
            set
            {
                qrcode.Text = value;
            }
        }

        public string Desc
        {
            set { desc.Text = value; }
        }
    }
}
