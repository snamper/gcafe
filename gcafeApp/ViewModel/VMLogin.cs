using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class VMLogin : VMBase
    {
        Action<Exception> _callBack;
        gcafeSvc.Staff _staff = new gcafeSvc.Staff();
        gcafeSvc.IgcafeSvcClient _svc;

        public VMLogin(IgcafeSvcClient svc)
        {
            IsUserError = false;
            IsPasswordError = false;

            _svc = svc;
            _svc.GetStaffByNumCompleted += _svc_GetStaffByNumCompleted;
        }

        public Action<Exception> ExceptionCallback
        {
            get { return _callBack; }
            set { _callBack = value; }
        }

        void _svc_GetStaffByNumCompleted(object sender, gcafeSvc.GetStaffByNumCompletedEventArgs e)
        {
            try
            {
                _staff = e.Result;
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
            catch (Exception ex)
            {
                if (_callBack != null)
                    _callBack(ex);
            }

            IsBusy = false;
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
                if (!string.IsNullOrEmpty(value))
                {
                    IsBusy = true;
                    _svc.GetStaffByNumAsync(Settings.AppSettings.DeviceID, value);
                    this._loginStaffNo = value;
                    RaisePropertyChanged();
                }
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

        public void Reset()
        {
            _loginStaffNo = "";
            _loginStaffPassword = "";
            IsPasswordError = false;
            _staff = null;
        }
    }
}
