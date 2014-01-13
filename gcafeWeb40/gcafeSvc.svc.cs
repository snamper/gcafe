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

namespace gcafeWeb40
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "gcafeSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select gcafeSvc.svc or gcafeSvc.svc.cs at the Solution Explorer and start debugging.
    public class gcafeSvc : IgcafeSvc
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        //private static gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient();

        private static string TraceMessage(
            string memberName = "",
            int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }

        public List<MenuCatalog> GetMenuCatalogs(string DeviceId, string rootCata)
        {
            List<MenuCatalog> cataList = new List<MenuCatalog>();

            _log.Trace(TraceMessage());

            try
            {
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

            _log.Trace(TraceMessage());

            try
            {
                switch (oprType)
                {
                    case TableOprType.OpenTable:
                        rtn = "开台成功";
                        break;

                    case TableOprType.ChangeTable:
                        break;

                    case TableOprType.ChangeCustomerNum:
                        break;
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
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return true;
        }

        public List<Method> GetMethods()
        {
            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return new List<Method>();
        }

        public List<MethodCatalog> GetMethodCatalogs()
        {
            List<MethodCatalog> rtn = new List<MethodCatalog>();

            _log.Trace(TraceMessage());

            try
            {
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
                using (gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient())
                {
                    //_gcafePrn.PrintChuPing(orderId, -1, false);
                    //_gcafePrn.PrintHuaDan(orderId, -1, false);
                    //_gcafePrn.PrintLiuTai(orderId, -1, false);
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

        public List<MenuItem> GetOrderDetailByOrderNum(string orderNum)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            //return menuItems;

            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuItems;
        }

        public Staff GetStaffByNum(string DeviceId, string Num)
        {
            _log.Trace(TraceMessage());

            try
            {
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
