using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace gcafeApp.ViewModel
{
    public class VMLogin : VMBase
    {
        public VMLogin()
        {

        }

        public int LoginStaffID
        {
            get { return _loginStaffID; }
            set
            {
                this._loginStaffID = value;
                RaisePropertyChanged();
            }
        }
        private int _loginStaffID;

        public string LoginStaffName
        {
            get { return _loginStaffName; }
            set
            {
                this._loginStaffName = value;
                RaisePropertyChanged();
            }
        }
        private string _loginStaffName;

        public string LoginStaffNo
        {
            get { return _loginStaffNo; }
            set
            {
                this._loginStaffNo = value;
                RaisePropertyChanged();
            }
        }
        private string _loginStaffNo;

        public string LoginStaffPassword
        {
            get { return _loginStaffPassword; }
            set
            {
                this._loginStaffPassword = value;
                RaisePropertyChanged();
            }
        }
        private string _loginStaffPassword;

        public bool IsLogin { get; set; }
    }
}
