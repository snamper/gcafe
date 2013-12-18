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
        private readonly IgcafeSvcClient _svc;

        //gcafeSvc.IgcafeSvcClient _svc = new IgcafeSvcClient();

        public VMMenuItem(IgcafeSvcClient svc)
        {
            _items = new ObservableCollection<MenuItem>();

            _svc = svc;
            _svc.GetMenuItemsByCatalogIdCompleted += _svc_GetMenuItemsByCatalogIdCompleted;
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            _svc.GetMenuItemsByCatalogIdCompleted -= _svc_GetMenuItemsByCatalogIdCompleted;
        }

        void _svc_GetMenuItemsByCatalogIdCompleted(object sender, GetMenuItemsByCatalogIdCompletedEventArgs e)
        {
            IsBusy = false;

            if (e.Result.GetMenuItemsByCatalogIdResult != null)
                this.Items = new ObservableCollection<MenuItem>(e.Result.GetMenuItemsByCatalogIdResult);
            else
                this.Items = new ObservableCollection<MenuItem>();
        }

        public int CatalogID
        {
            set
            {
                IsBusy = true;

                GetMenuItemsByCatalogIdRequest req = new GetMenuItemsByCatalogIdRequest(Settings.AppSettings.DeviceID, value);
                _svc.GetMenuItemsByCatalogIdAsync(req);
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
