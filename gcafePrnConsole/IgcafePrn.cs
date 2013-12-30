using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gcafeSvc
{
    [ServiceContract]
    public interface IgcafePrn
    {
        /// <summary>
        /// 留台单打印
        /// </summary>
        /// <param name="orderId">要打印的order id</param>
        /// <param name="prnType">打印的类型，-1 = 最后一次的点菜，0 = 所有的点菜，> 1的代表要打印第几次的点菜</param>
        /// <returns></returns>
        [OperationContract]
        string PrintLiuTai(int orderId, int prnType);

        /// <summary>
        /// 划单打印
        /// </summary>
        /// <param name="orderId">要打印的order id</param>
        /// <param name="prnType">打印的类型，-1 = 最后一次的点菜，0 = 所有的点菜，> 1的代表要打印第几次的点菜</param>
        /// <returns></returns>
        [OperationContract]
        string PrintHuaDan(int orderId, int prnType);

        /// <summary>
        /// 出品单打印
        /// </summary>
        /// <param name="orderId">要打印的order id</param>
        /// <param name="prnType">打印的类型，-1 = 最后一次的点菜，0 = 所有的点菜，> 1的代表要打印第几次的点菜</param>
        /// <returns></returns>
        [OperationContract]
        string PrintChuPing(int orderId, int prnType);

        /// <summary>
        /// 出品单打印，催单时用
        /// </summary>
        /// <param name="orderId">要打印的order id</param>
        /// <param name="orderDetailId">要催的orderDetailId，如果整台催，-1</param>
        /// <param name="setmailId">要催的setmailId, -1代表忽略这参数</param>
        /// <returns></returns>
        [OperationContract]
        string PrintChuPingCui(int orderId, int orderDetailId, int setmailId);
    }
}
