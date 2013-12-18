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

        public VMSubMenuCatalog()
        {
            this.Items = new ObservableCollection<MenuCatalog>();

            if (!DesignerProperties.IsInDesignTool)
            {
                _svc = new IgcafeSvcClient();
                _svc.GetMenuCatalogsCompleted += _svc_GetMenuCatalogsCompleted;
            }
        }

        void _svc_GetMenuCatalogsCompleted(object sender, GetMenuCatalogsCompletedEventArgs e)
        {
            IsBusy = false;

            if (e.Result.GetMenuCatalogsResult != null)
                this.Items = new ObservableCollection<MenuCatalog>(e.Result.GetMenuCatalogsResult);
            else
                this.Items = new ObservableCollection<MenuCatalog>();
        }

        public string Catalog
        {
            set
            {
                IsBusy = true;

                GetMenuCatalogsRequest req = new GetMenuCatalogsRequest(Settings.AppSettings.DeviceID, value);
                _svc.GetMenuCatalogsAsync(req);
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
