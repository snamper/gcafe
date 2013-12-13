using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GalaSoft.MvvmLight;

namespace gcafeApp.Controls
{
    public partial class PageTitle : UserControl
    {
        public PageTitle()
        {
            InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                if (Settings.AppSettings.LoginStaff != null)
                    _loginStaff.Text = Settings.AppSettings.LoginStaff.Name;
                else
                    _loginStaff.Text = "未登录";
            }
        }
    }
}
