using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class VMSubMenuCatalog : VMBase
    {
        gcafeSvc.IgcafeSvcClient _svc;

        public VMSubMenuCatalog(IgcafeSvcClient svc)
        {
            this.Items = new ObservableCollection<MenuCatalog>();

            if (!IsInDesignMode)
            {
                _svc = svc;
                _svc.GetMenuCatalogsCompleted += _svc_GetMenuCatalogsCompleted;
            }
        }

        void _svc_GetMenuCatalogsCompleted(object sender, GetMenuCatalogsCompletedEventArgs e)
        {
            IsBusy = false;

            try
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                        this.Items = new ObservableCollection<MenuCatalog>(e.Result);
                    else
                        this.Items = new ObservableCollection<MenuCatalog>();
                }
                else
                {
                    System.Windows.MessageBox.Show(e.Error.Message);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public string Catalog
        {
            set
            {
                IsBusy = true;

                this.Items = new ObservableCollection<MenuCatalog>();
                _svc.GetMenuCatalogsAsync(Settings.AppSettings.DeviceID, value);
            }
        }

        public ObservableCollection<MenuCatalog> Items 
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<MenuCatalog> _items;
    }
}
