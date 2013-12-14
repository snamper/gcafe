using System;
using System.Windows.Input;
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
    public class VMMenuItem :　VMBase
    {
        gcafeSvc.IgcafeSvcClient _svc = new IgcafeSvcClient();

        public VMMenuItem()
        {
            _items = new ObservableCollection<MenuItem>();
            _svc.GetMenuItemByCatalogIdCompleted += _svc_GetMenuItemByCatalogIdCompleted;
        }

        void _svc_GetMenuItemByCatalogIdCompleted(object sender, GetMenuItemByCatalogIdCompletedEventArgs e)
        {
            IsBusy = false;

            if (e.Result.GetMenuItemByCatalogIdResult != null)
                this.Items = new ObservableCollection<MenuItem>(e.Result.GetMenuItemByCatalogIdResult);
            else
                this.Items = new ObservableCollection<MenuItem>();
        }

        public int CatalogID
        {
            set
            {
                IsBusy = true;

                GetMenuItemByCatalogIdRequest req = new GetMenuItemByCatalogIdRequest(value);
                _svc.GetMenuItemByCatalogIdAsync(req);
            }
        }

        public ObservableCollection<MenuItem> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<MenuItem> _items;

    }
}
