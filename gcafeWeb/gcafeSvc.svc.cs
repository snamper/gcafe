using System;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;

namespace gcafeWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“gcafeSvc”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 gcafeSvc.svc 或 gcafeSvc.svc.cs，然后开始调试。
    public class gcafeSvc : IgcafeSvc
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        //private static gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient();

        private static string TraceMessage(
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }

        public List<MenuCatalog> GetMenuCatalogs(string DeviceId, string rootCata)
        {
            List<MenuCatalog> cataList = new List<MenuCatalog>();

            _log.Trace(TraceMessage());

            try
            {
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
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return cataList;
        }

        public List<MenuItem> GetMenuItemsByCatalogId(string DeviceId, int cataId)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            List<MenuItem> menuList = new List<MenuItem>();

            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    List<menu> menus = context.menu
                        .Include(n => n.setmeal_item.Select(m => m.menu1))
                        .Include(n => n.setmeal_item.Select(m => m.setmeal_item_opt))
                        .Where(n => (n.menu_catalog_id == cataId) && (n.branch_id == branchId))
                        .OrderBy(n => n.name)
                        .ToList();

                    foreach (menu menu in menus)
                    {
                        if (menu.setmeal_item.Count > 0)
                        {
                            List<SetmealItem> setmeals = new List<SetmealItem>();
                            foreach (var smitem in menu.setmeal_item)
                            {
                                //gcafeWeb.menu m = context.menu.FirstOrDefault(n => n.id == smitem.setmeal_item_menu_id);

                                if (smitem.setmeal_item_opt.Count > 0)
                                {
                                    List<SetmealItem> opts = new List<SetmealItem>();
                                    foreach (var opt in smitem.setmeal_item_opt)
                                    {
                                        //gcafeWeb.menu m1 = context.menu.FirstOrDefault(n => n.id == opt.menu_id);
                                        opts.Add(new SetmealItem() { MenuID = opt.menu.id, Name = opt.menu.name, Unit = opt.menu.unit });
                                    }
                                    setmeals.Add(new SetmealItem() { MenuID = smitem.menu1.id, Name = smitem.menu1.name, Unit = smitem.menu1.unit, OptionItems = opts });
                                }
                                else
                                    setmeals.Add(new SetmealItem() { MenuID = smitem.menu1.id, Name = smitem.menu1.name, Unit = smitem.menu1.unit });
                            }

                            menuList.Add(new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = true, Quantity = 1, SetmealItems = setmeals });
                        }
                        else
                            menuList.Add(new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = false, Quantity = 1 });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuList;
        }

        public MenuItem GetMenuItemByNumber(string DeviceId, string number)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    menu menu = context.menu.Where(n => n.number == number && n.branch_id == branchId).FirstOrDefault();
                    if (menu != null)
                        return new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = menu.is_setmeal };
                    else
                        return (MenuItem)null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return (MenuItem)null;
        }

        public string TableOpr(string DeviceId, TableInfo tableInfo, string oldTableNum, TableOprType oprType)
        {
            string rtn = string.Empty;

            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);

            order order;
            device dev;

            _log.Trace(TraceMessage());

            try
            {
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
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public List<TableInfo> GetTablesInfo(string DeviceId)
        {
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);
            List<TableInfo> tableInfoList = new List<TableInfo>();

            _log.Trace(TraceMessage());

            try
            {
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
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return tableInfoList;
        }

        public bool IsTableAvaliable(string tableNum)
        {
            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    if (context.order.Where(n => n.table_no == tableNum && n.check_out_staff_id == null).FirstOrDefault() != null)
                        return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return true;
        }

        public List<method> GetMethods()
        {
            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    return context.method.Include(n => n.method_catalog).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return new List<method>();
        }

        public List<MethodCatalog> GetMethodCatalogs()
        {
            List<MethodCatalog> rtn = new List<MethodCatalog>();

            _log.Trace(TraceMessage());

            try
            {
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
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public string OrderMeal(string deviceId, int staffId, string tableNum, List<MenuItem> meals)
        {
            string rtn = string.Empty;
            int orderId;

            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    device dev = context.device.FirstOrDefault(n => n.device_id == deviceId && n.is_deny == false);
                    order order = context.order.FirstOrDefault(n => n.table_no == tableNum && n.check_out_staff_id == null);

                    if (dev == null)
                        return "设备未验证";

                    if (order == null)
                        return string.Format("{0} 台还没开", tableNum);
                    else
                        orderId = order.id;

                    int groupCnt = context.sys_info.FirstOrDefault().order_detail_cnt++;
                    context.SaveChanges();

                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var meal in meals)
                        {
                            order_detail orderDetail = new order_detail()
                            {
                                device_id = dev.id,
                                order_id = order.id,
                                menu_id = meal.ID,
                                group_cnt = groupCnt + 1,
                                price = meal.Price,
                                quantity = meal.Quantity,
                                order_staff_id = staffId,
                                order_time = System.DateTime.Now,
                            };
                            context.order_detail.Add(orderDetail);
                            context.SaveChanges();

                            if (meal.Methods != null)
                            {
                                foreach (var method in meal.Methods)
                                    context.order_detail_method.Add(new order_detail_method() { order_detail_id = orderDetail.id, method_id = method.id });

                                context.SaveChanges();
                            }

                            if (meal.SetmealItems != null)
                            {
                                foreach (var setmeal in meal.SetmealItems)
                                {
                                    order_detail_setmeal ods = new order_detail_setmeal()
                                    {
                                        order_detail_id = orderDetail.id,
                                        menu_id = setmeal.MenuID,
                                    };
                                    context.order_detail_setmeal.Add(ods);
                                    context.SaveChanges();

                                    if (setmeal.Methods != null)
                                    {
                                        foreach (var method in setmeal.Methods)
                                            context.order_detail_method.Add(new order_detail_method() { order_detail_setmeal_id = ods.id, method_id = method.id });

                                        context.SaveChanges();
                                    }
                                }
                            }
                        }

                        scope.Complete();
                    }

                    using (gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient())
                    {
                        _gcafePrn.PrintChuPing(orderId, -1, false);
                        _gcafePrn.PrintHuaDan(orderId, -1, false);
                        _gcafePrn.PrintLiuTai(orderId, -1, false);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));

                //if (_gcafePrn.State == CommunicationState.Faulted)
                //   _gcafePrn = new gcafePrnSvc.IgcafePrnClient();
            }

            _log.Trace(TraceMessage());

            return "点菜成功";
        }

        public List<order_detail> GetOrderDetailByOrderNum(string orderNum)
        {
            List<order_detail> orderDetails = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var context = new gcafeEntities())
                {
                    orderDetails = context.order_detail
                        .Include("menu")
                        .Include(n => n.order_detail_method.Select(m => m.method))
                        .Include(n => n.order_detail_setmeal.Select(m => m.menu))
                        .Where(n => n.order.order_num == orderNum)
                        .ToList();
                }

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return orderDetails;
        }

        public Staff GetStaffByNum(string DeviceId, string Num)
        {
            _log.Trace(TraceMessage());

            try
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
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return null;
        }
    }
}
