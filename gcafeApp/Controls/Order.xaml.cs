using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace gcafeApp.Controls
{
    public partial class Order : UserControl
    {
        public Order()
        {
            InitializeComponent();
        }

        public string TableNum
        {
            get { return tblPicker.Value; }
            set
            {
                tblPicker.Value = value;
            }
        }
    }
}
