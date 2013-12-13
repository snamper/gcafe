using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace gcafeApp.ViewModel
{
    public class VMMenuCatalog : VMBase
    {
        private RelayCommand<string> navigateToCommand;

        public VMMenuCatalog()
        {
            navigateToCommand = new RelayCommand<string>(NavigateTo);
        }

        private void NavigateTo(string cata)
        {

        }
    }
}
