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
        /// <summary>
        /// 从menu_catalog按rootCata取数据
        /// </summary>
        /// <param name="rootCata">厨房，酒吧</param>
        /// <returns></returns>
        [OperationContract]
        List<MenuCatalog> GetMenuCatalogs(string DeviceId, string rootCata);

        /// <summary>
        /// 根据cataId取数据
        /// </summary>
        /// <param name="cataId"></param>
        /// <returns></returns>
        [OperationContract]
        List<MenuItem> GetMenuItemsByCatalogId(string DeviceId, int cataId);

        /// <summary>
        /// 根据number取菜单项
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [OperationContract]
        MenuItem GetMenuItemByNumber(string DeviceId, string number);

        /// <summary>
        /// 餐台操作，开台，换台，改人数都在这里操作
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="tableNum"></param>
        /// <param name="oldTableNum"></param>
        /// <param name="customerNum"></param>
        /// <param name="oprType"></param>
        /// <returns>成功返回"成功", 失败返回出错原因</returns>
        [OperationContract]
        string TableOpr(string DeviceId, TableInfo tableInfo, string oldTableNum, TableOprType oprType);

        /// <summary>
        /// 取当前已开台并且还没埋单的台的信息
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        [OperationContract]
        List<TableInfo> GetTablesInfo(string DeviceId);

        /// <summary>
        /// 检查tableNum的台是否可以开台
        /// </summary>
        /// <param name="tableNum"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsTableAvaliable(string tableNum);

        [OperationContract]
        List<method> GetMethods();

        [OperationContract]
        List<MethodCatalog> GetMethodCatalogs();

        [OperationContract]
        string OrderMeal(string deviceId, int staffId, string tableNum, List<MenuItem> meals);

        /// <summary>
        /// 根据Num取员工信息
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        [OperationContract]
        Staff GetStaffByNum(string DeviceId, string Num);
    }

}
