using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using gcafeApp.Controls;

namespace gcafeApp.Pages
{
    public partial class TableNoPage : DataPickerPageBase
    {
        public TableNoPage()
        {
            InitializeComponent();

            InitializeDataPickerPage();
        }

        /// <summary>
        /// Sets the selectors and title flow direction.
        /// </summary>
        /// <param name="flowDirection">Flow direction to set.</param>
        public override void SetFlowDirection(FlowDirection flowDirection)
        {
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (_pivot.SelectedIndex == 1)
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
                if (btn.Text == "确定")
                {
                    ViewModel.TablesViewModel vm = (ViewModel.TablesViewModel)DataContext;
                    vm.OpenTable();
                }
            }
        }
    }
}