using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace gcafeApp.Pages
{
    public partial class MenuCameraSelect : PhoneApplicationPage
    {
        public MenuCameraSelect()
        {
            InitializeComponent();

            ((ViewModel.VMMenuCameraSelect)DataContext).PropertyChanged += MenuCameraSelect_PropertyChanged;
        }

        void MenuCameraSelect_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.PropertyName);

            if (e.PropertyName == "SelectedMenuItem")
                NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ((ViewModel.VMMenuCameraSelect)DataContext).Uninitialize();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ((ViewModel.VMMenuCameraSelect)DataContext).InitializeAndGo();
        }

    }
}