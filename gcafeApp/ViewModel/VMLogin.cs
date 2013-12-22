using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace gcafeApp.ViewModel
{
    public class VMLogin : VMBase
    {
        gcafeSvc.Staff1 _staff = new gcafeSvc.Staff1();
        gcafeSvc.IgcafeSvcClient _svc = new gcafeSvc.IgcafeSvcClient();

        public VMLogin()
        {
            IsUserError = false;
            IsPasswordError = false;

            _svc.GetStaffByNumCompleted += _svc_GetStaffByNumCompleted;
        }

        void _svc_GetStaffByNumCompleted(object sender, gcafeSvc.GetStaffByNumCompletedEventArgs e)
        {
            _staff = e.Result.GetStaffByNumResult;
            if (_staff == null)
            {
                IsUserError = true;
            }
            else
            {
                IsUserError = false;
                IsPasswordError = false;
            }
        }

        public gcafeSvc.Staff1 Staff
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
                System.Diagnostics.Debug.WriteLine("LoginStaffPassword");

                this._loginStaffPassword = value;
                RaisePropertyChanged();

                if (this._loginStaffPassword == _staff.Password)
                {
                    IsLogin = true;
                    IsPasswordError = false;
                }
                else
                {
                    IsLogin = false;
                    IsPasswordError = true;
                }
            }
        }
        private string _loginStaffPassword;

        public bool IsUserError
        {
            get { return _isUserError; }
            set
            {
                this._isUserError = value;
                RaisePropertyChanged();
            }
        }
        private bool _isUserError;

        public bool IsPasswordError
        {
            get { return _isPasswordError; }
            set
            {
                this._isPasswordError = value;
                RaisePropertyChanged();
            }
        }
        private bool _isPasswordError;

        public bool IsLogin { get; set; }
    }
}
