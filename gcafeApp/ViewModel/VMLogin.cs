using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace gcafeApp.ViewModel
{
    public class VMLogin : VMBase
    {
        gcafeSvc.Staff _staff = new gcafeSvc.Staff();
        gcafeSvc.IgcafeSvcClient _svc = new gcafeSvc.IgcafeSvcClient();

        public VMLogin()
        {
            _svc.GetStaffByNumCompleted += _svc_GetStaffByNumCompleted;
        }

        void _svc_GetStaffByNumCompleted(object sender, gcafeSvc.GetStaffByNumCompletedEventArgs e)
        {
            _staff = e.Result.GetStaffByNumResult;
        }

        public gcafeSvc.Staff Staff
        {
            get { return _staff; }
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
                gcafeSvc.GetStaffByNumRequest req = new gcafeSvc.GetStaffByNumRequest();
                req.Num = value;
                _svc.GetStaffByNumAsync(req);
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

                if (this._loginStaffPassword == _staff.Password)
                    IsLogin = true;
                else
                    IsLogin = false;
            }
        }
        private string _loginStaffPassword;

        public bool IsLogin { get; set; }
    }
}
