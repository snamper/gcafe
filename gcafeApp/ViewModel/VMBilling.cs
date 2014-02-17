using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        public string OrderNum
        {
            get { return _orderNum; }
            set
            {
                if (_orderNum != value)
                {
                    _orderNum = value;

                    RaisePropertyChanged();
                }
            }
        }
        private string _orderNum;

        public List<MenuItem> MenuItems { get; set; }

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

                    DiscountedPrice = _orderDetail.Amount;

                    var bmp = writer.Write(string.Format("{0}", DiscountedPrice));
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

        public string DiscountStr
        {
            get { return _discountStr; }
            private set
            {
                _discountStr = value;
                RaisePropertyChanged();
            }
        }
        string _discountStr = "无打折";

        int Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                if (_discount == 100)
                {
                    DiscountStr = "无打折";
                    DiscountedPrice = OrderDetail.Amount;
                }
                else
                {
                    DiscountStr = string.Format("{0}折", _discount.ToString());
                    DiscountedPrice = (decimal)0;
                    foreach (var menuItem in MenuItems)
                        DiscountedPrice += menuItem.DiscountAllowed ? menuItem.Price * (_discount / 100) : menuItem.Price;
                }
            }
        }
        int _discount = 100;

        public decimal DiscountedPrice
        {
            get { return _discountedPrice; }
            private set
            {
                _discountedPrice = value;
                RaisePropertyChanged();

                var format = new BarcodeFormat();
                var option = new EncodingOptions() { Height = 500, Width = 500 };
                BarcodeFormat.TryParse("QR_CODE", out format);
                IBarcodeWriter writer = new BarcodeWriter
                {
                    Format = format,
                    Options = option,
                };

                var bmp = writer.Write(string.Format("{0}", DiscountedPrice));
                ImageSource = bmp;
            }
        }
        decimal _discountedPrice;

        public RelayCommand<string> DiscountCommand
        {
            get
            {
                if (_discountCommand == null)
                {
                    _discountCommand = new RelayCommand<string>(sign =>
                    {
                        if (sign == "+")
                        {
                            if (Discount < 100)
                                Discount += 5;
                        }
                        else
                        {
                            if (Discount > 10)
                                Discount -= 5;
                        }
                    });
                }

                return _discountCommand;
            }
        }
        RelayCommand<string> _discountCommand;

    }
}
