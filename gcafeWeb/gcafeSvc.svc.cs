using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gcafeWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“gcafeSvc”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 gcafeSvc.svc 或 gcafeSvc.svc.cs，然后开始调试。
    public class gcafeSvc : IgcafeSvc
    {
        public List<MenuCatalog> GetMenuCatalogs(string DeviceId, string rootCata)
        {
            List<MenuCatalog> cataList = new List<MenuCatalog>();

            using (var context = new gcafeEntities())
            {
                int parent_id = 0;

                if (rootCata == "厨房")
                    parent_id = 1;
                else if (rootCata == "酒吧")
                    parent_id = 2;

                List<menu_catalog> menuCataList = context.menu_catalog.Where(n => n.parent_id == parent_id).OrderBy(n => n.name).ToList();
                foreach (menu_catalog mc in menuCataList)
                {
                    cataList.Add(new MenuCatalog() { ID = mc.id, Name = mc.name });
                }
            }

            return cataList;
        }

        public List<MenuItem> GetMenuItemsByCatalogId(string DeviceId, int cataId)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            List<MenuItem> menuList = new List<MenuItem>();

            using (var context = new gcafeEntities())
            {
                List<menu> menus = context.menu.Include(n => n.setmeal_item1).Where(n => (n.menu_catalog_id == cataId) && (n.branch_id == branchId)).OrderBy(n => n.name).ToList();
                foreach (menu menu in menus)
                {
                    if (menu.setmeal_item.Count > 0)
                    {
                        List<SetmealItem> setmeals = new List<SetmealItem>();
                        foreach (var smitem in menu.setmeal_item)
                        {
                            gcafeWeb.menu m = context.menu.FirstOrDefault(n => n.id == smitem.setmeal_item_menu_id);
                            setmeals.Add(new SetmealItem() { Name = m.name, Unit = menu.unit });

                            if (smitem.setmeal_item_opt.Count > 0)
                            {
                                int i = 0;
                            }
                        }

                        menuList.Add(new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = true, Quantity = 1, SetmealItems = setmeals });
                    }
                    else
                        menuList.Add(new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = false, Quantity = 1 });
                }
            }

            return menuList;
        }

        public MenuItem GetMenuItemByNumber(string DeviceId, string number)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            using (var context = new gcafeEntities())
            {
                menu menu = context.menu.Where(n => n.number == number && n.branch_id == branchId).FirstOrDefault();
                if (menu != null)
                    return new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = menu.is_setmeal };
                else
                    return (MenuItem)null;
            }
        }

        public string TableOpr(string DeviceId, TableInfo tableInfo, string oldTableNum, TableOprType oprType)
        {
            string rtn = string.Empty;

            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            order order;
            device dev;

            using (var context = new gcafeEntities())
            {
                switch (oprType)
                {
                    case TableOprType.OpenTable:
                        dev = context.device.Where(n => n.device_id == DeviceId && n.is_deny == false).FirstOrDefault();
                        if (dev == null)
                            rtn = "设备未验证";
                        else
                        {
                            sys_info sysInfo = context.sys_info.FirstOrDefault();
                            string orderNum = string.Format("{0}{1}{2}{3:D2}{4:D4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, ++sysInfo.order_cnt);

                            order = new order() 
                            { 
                                branch_id = branchId, 
                                device_id = dev.id, 
                                order_num = orderNum,
                                table_no = tableInfo.Num, 
                                customer_number = tableInfo.CustomerNum, 
                                open_table_staff_id = tableInfo.OpenTableStaff.ID,
                                table_opened_time = DateTime.Now,
                            };

                            context.order.Add(order);
                            context.SaveChanges();

                            rtn = "开台成功";
                        }
                        break;

                    case TableOprType.ChangeTable:
                        break;

                    case TableOprType.ChangeCustomerNum:
                        break;
                }
            }

            return rtn;
        }

        public List<TableInfo> GetTablesInfo(string DeviceId)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);
            List<TableInfo> tableInfoList = new List<TableInfo>();

            using (var context = new gcafeEntities())
            {
                List<order> orders = context.order.Include("staff2").Where(n => n.branch_id == branchId && n.check_out_staff_id == null).ToList();
                foreach (var order in orders)
                {
                    tableInfoList.Add(new TableInfo()
                    {
                        ID = order.id,
                        OrderNum = order.order_num,
                        Num = order.table_no,
                        CustomerNum = order.customer_number,
                        Amount = order.receivable == null ? 0 : (decimal)order.receivable,
                        OpenTableStaff = new Staff() { Name = order.staff2.name },
                        OpenTableTime = order.table_opened_time,
                    });
                }
            }

            return tableInfoList;
        }

        public bool IsTableAvaliable(string tableNum)
        {
            using (var context = new gcafeEntities())
            {
                if (context.order.Where(n => n.table_no == tableNum && n.check_out_staff_id == null).FirstOrDefault() != null)
                    return false;
            }

            return true;
        }

        public List<method> GetMethods()
        {
            using (var context = new gcafeEntities())
            {
                return context.method.Include(n => n.method_catalog).ToList();
            }
        }

        public List<MethodCatalog> GetMethodCatalogs()
        {
            List<MethodCatalog> rtn = new List<MethodCatalog>();

            using (var context = new gcafeEntities())
            {
                List<method_catalog> methodCatalogs = context.method_catalog.Include(n => n.method).ToList();

                foreach (var methodCatalog in methodCatalogs)
                {
                    List<Method> methods = new List<Method>();
                    foreach (var method in methodCatalog.method)
                    {
                        methods.Add(new Method() { ID = method.id, Name = method.name });
                    }

                    rtn.Add(new MethodCatalog() { ID = methodCatalog.id, Name = methodCatalog.name, Methods = methods });
                }
            }

            return rtn;
        }


        public Staff GetStaffByNum(string DeviceId, string Num)
        {
            using (var context = new gcafeEntities())
            {
                staff staff = context.staff.Include("branch").Where(n => n.number == Num).FirstOrDefault();

                if (staff != null)
                {
                    Staff stf = new Staff() { Name = staff.name, ID = staff.id, Number = staff.number, Password = staff.password, RoleID = staff.role_id, Branch = staff.branch.name, BranchID = staff.branch_id };
                    return stf;
                }
                else
                    return null;
            }
        }
    }
}
