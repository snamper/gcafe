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
    public class VMMenuOption : VMBase
    {
        public VMMenuOption()
        {

        }

        public List<SetmealItem> OptionItems
        {
            get { return _optionItems; }
            set
            {
                if (!ReferenceEquals(value, _optionItems))
                {
                    _optionItems = value;
                    RaisePropertyChanged();
                }
            }
        }
        private List<SetmealItem> _optionItems;
    }
}
