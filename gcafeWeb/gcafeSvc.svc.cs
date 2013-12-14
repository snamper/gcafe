using System;
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
        public List<MenuCatalog> GetMenuCatalogs(string rootCata)
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

        public List<MenuItem> GetMenuItemsByCatalogId(int branchId, int cataId)
        {
            List<MenuItem> menuList = new List<MenuItem>();

            using (var context = new gcafeEntities())
            {
                List<menu> menus = context.menu.Where(n => (n.menu_catalog_id == cataId) && (n.branch_id == branchId)).OrderBy(n => n.name).ToList();
                foreach (menu menu in menus)
                {
                    menuList.Add(new MenuItem() { ID = menu.id, Name = menu.name, Unit = menu.unit, Price = menu.price, IsSetmeal = menu.is_setmeal });
                }
            }

            return menuList;
        }


        public Staff GetStaffByNum(string Num)
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
