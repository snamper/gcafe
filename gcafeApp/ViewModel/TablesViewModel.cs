using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace gcafeApp.ViewModel
{
    public class TablesViewModel : ViewModelBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public TablesViewModel()
        {
            this.Items = new ObservableCollection<TableViewModel>();
        }

        public ObservableCollection<TableViewModel> Items { get; private set; }
    }
}
