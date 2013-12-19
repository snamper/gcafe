﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class TablesViewModel : VMBase
    {
        private readonly IgcafeSvcClient _svc;

        /// <summary>
        /// 初始化
        /// </summary>
        public TablesViewModel(IgcafeSvcClient svc)
        {
            this.Items = new ObservableCollection<TableViewModel>();

            if (!DesignerProperties.IsInDesignTool)
            {
                _svc = svc;
                _svc.TableOprCompleted += _svc_TableOprCompleted;
                _svc.GetTablesInfoCompleted += _svc_GetTablesInfoCompleted;
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            _svc.GetTablesInfoCompleted -= _svc_GetTablesInfoCompleted;
            _svc.TableOprCompleted -= _svc_TableOprCompleted;
        }

        void _svc_GetTablesInfoCompleted(object sender, GetTablesInfoCompletedEventArgs e)
        {
            if (e.Result.GetTablesInfoResult != null)
            {
                List<TableViewModel> vmList = new List<TableViewModel>();
                foreach (TableInfo tbl in e.Result.GetTablesInfoResult)
                {
                    vmList.Add(new TableViewModel()
                    {
                        TableNo = tbl.Num,
                        CustomerNum = tbl.CustomerNum,
                        OpenTableStaff = tbl.OpenTableStaff.Name,
                        TableOpenedTime = tbl.OpenTableTime,
                        Amount = tbl.Amount
                    });
                }
                this.Items = new ObservableCollection<TableViewModel>(vmList);
            }
        }

        void _svc_TableOprCompleted(object sender, TableOprCompletedEventArgs e)
        {
            string s = e.Result.TableOprResult;
        }

        /// <summary>
        /// 已开台的列表
        /// </summary>
        public ObservableCollection<TableViewModel> Items 
        { 
            get { return _items; }
            private set
            {
                if (!ReferenceEquals(_items, value))
                {
                    _items = value;
                    RaisePropertyChanged();
                }
            }
        }
        private ObservableCollection<TableViewModel> _items;

        /// <summary>
        /// 台号
        /// </summary>
        public string TableNum
        {
            get { return _tableNum; }
            set
            {
                if (!ReferenceEquals(value, _tableNum))
                {
                    _tableNum = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _tableNum;

        /// <summary>
        /// 客人数目
        /// </summary>
        public int CustomerNum
        {
            get { return _customerNum; }
            set
            {
                if (_customerNum != value)
                {
                    _customerNum = value;
                    RaisePropertyChanged();
                }
            }
        }
        private int _customerNum;

        public void OpenTable()
        {
            TableOprRequest req = new TableOprRequest() 
            { 
                DeviceId = Settings.AppSettings.DeviceID, 
                tableInfo = new TableInfo() { Num = TableNum, CustomerNum = CustomerNum, OpenTableStaff = Settings.AppSettings.LoginStaff }, 
                oprType = TableOprType.OpenTable 
            };

            _svc.TableOprAsync(req);
        }
    }
}
