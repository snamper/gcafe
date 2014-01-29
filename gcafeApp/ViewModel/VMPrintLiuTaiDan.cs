using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight.Messaging;

namespace gcafeApp.ViewModel
{
    public class VMPrintLiuTaiDan : VMBase
    {
        IgcafeSvcClient _svc;

        public VMPrintLiuTaiDan(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;
            }
        }
    }
}
