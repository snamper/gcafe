using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcafeApp.Helper
{
    public static class ObjectExtensions
    {
        public static gcafeSvc.MenuItem Copy(this gcafeSvc.MenuItem instance)
        {
            if (instance == null)
                return null;
            gcafeSvc.MenuItem rtn = new gcafeSvc.MenuItem() 
            {
                ID = instance.ID,
                DiscountAllowed = instance.DiscountAllowed,
                GroupCnt = instance.GroupCnt,
                IsSetmeal = instance.IsSetmeal,
                //Methods = new System.Collections.ObjectModel.ObservableCollection<gcafeSvc.Method>(instance.Methods),
                Name = instance.Name,
                Number = instance.Number,
                OrderStaffName = instance.OrderStaffName,
                OrderTime = instance.OrderTime,
                Price = instance.Price,
                ProduceTime = instance.ProduceTime,
                Quantity = instance.Quantity,
                //SetmealItems = new System.Collections.ObjectModel.ObservableCollection<gcafeSvc.SetmealItem>(instance.SetmealItems),
                Unit = instance.Unit,
            };

            if (instance.Methods != null)
                rtn.Methods = new System.Collections.ObjectModel.ObservableCollection<gcafeSvc.Method>(instance.Methods);
            if (instance.SetmealItems != null)
                rtn.SetmealItems = new System.Collections.ObjectModel.ObservableCollection<gcafeSvc.SetmealItem>(instance.SetmealItems);

            return rtn;
        }

    }
}
