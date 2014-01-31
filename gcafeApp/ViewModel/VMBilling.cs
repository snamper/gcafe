using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight.Messaging;
using ZXing;
using ZXing.Common;

namespace gcafeApp.ViewModel
{
    public class VMBilling : VMBase
    {
        IgcafeSvcClient _svc;

        public VMBilling(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;
            }
        }

        public TableViewModel OrderDetail
        {
            get { return _orderDetail; }
            set
            {
                if (!ReferenceEquals(_orderDetail, value))
                {
                    _orderDetail = value;

                    var format = new BarcodeFormat();
                    var option = new EncodingOptions() { Height = 500, Width = 500 };
                    BarcodeFormat.TryParse("QR_CODE", out format);
                    IBarcodeWriter writer = new BarcodeWriter
                    {
                        Format = format,
                        Options = option,
                    };

                    var bmp = writer.Write("hello");
                    ImageSource = bmp;

                    RaisePropertyChanged();
                }
            }
        }
        TableViewModel _orderDetail;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (!Equals(value, _imageSource))
                {
                    _imageSource = value;
                    RaisePropertyChanged();
                }
            }
        }
        ImageSource _imageSource;

    }
}
