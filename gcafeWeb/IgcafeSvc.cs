using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gcafeWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IgcafeSvc”。
    [ServiceContract]
    public interface IgcafeSvc
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        List<Order> GetOrders();

        [OperationContract]
        List<MenuItem> GetMenu();

        /// <summary>
        /// 从menu_catalog按rootCata取数据
        /// </summary>
        /// <param name="rootCata">厨房，酒吧</param>
        /// <returns></returns>
        [OperationContract]
        List<MenuCatalog> GetMenuCatalogs(string rootCata);

        /// <summary>
        /// 根据cataId取数据
        /// </summary>
        /// <param name="cataId">menu_catalog.id</param>
        /// <returns></returns>
        [OperationContract]
        List<MenuItem> GetMenuItemByCatalogId(int cataId);

        /// <summary>
        /// 根据Num取员工信息
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        [OperationContract]
        Staff GetStaffByNum(string Num);
    }

}
