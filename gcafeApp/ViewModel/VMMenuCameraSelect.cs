using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Devices;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using gcafeApp.gcafeSvc;
using ZXing;

namespace gcafeApp.ViewModel
{
    public class VMMenuCameraSelect : VMBase
    {
        private readonly IgcafeSvcClient _svc;
        bool _notfocus = false;

        public VMMenuCameraSelect(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;
                _svc.GetMenuItemByNumberCompleted += _svc_GetMenuItemByNumberCompleted;
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            _svc.GetMenuItemByNumberCompleted -= _svc_GetMenuItemByNumberCompleted;
        }

        void _svc_GetMenuItemByNumberCompleted(object sender, GetMenuItemByNumberCompletedEventArgs e)
        {
            try
            {
                if (e.Result.GetMenuItemByNumberResult != null)
                {
                    MenuItem menuItem = e.Result.GetMenuItemByNumberResult;
                    this.Result = menuItem.Name;
                    //System.Diagnostics.Debug.WriteLine(this.Result);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        public async void InitializeAndGo()
        {
            CaptureResolution = new Size(800, 480);
            await InitializePhotoCaptureDevice(CaptureResolution);
            await StartCapturingAsync();
            await PhotoCaptureDevice.FocusAsync();
            int i = 0;
            while (true)
            {
                Result result = await GetBarcodeAsync();

                if (result != null)
                {
                    GetMenuItemByNumberRequest req = new GetMenuItemByNumberRequest(Settings.AppSettings.DeviceID, result.Text);
                    _svc.GetMenuItemByNumberAsync(req);
                }
                //string s = result.Text;
                //if (result != null)
                //{
                //    System.Diagnostics.Debug.WriteLine(result);
                //    if (result.Length > 2)
                //    {
                //        System.Diagnostics.Debug.WriteLine(result);
                //        if (result.Substring(0, 2) == "11" || result.Substring(0, 2) == "22")
                //        {
                //            System.Diagnostics.Debug.WriteLine(result);
                //            Result = result;
                //            System.Diagnostics.Debug.WriteLine(result);
                //            if (i++ > 5)
                //                break;
                //        }
                //    }
                //}

                //result = "m";
            }

            PhotoCaptureDevice.Dispose();
            PhotoCaptureDevice = null;
        }

        private async Task<Result> GetBarcodeAsync()
        {
            if (!_notfocus)
                await PhotoCaptureDevice.FocusAsync();
            //PhotoCaptureDevice.FocusAsync();
            return await DetectBarcodeAsync();
        }

        private async Task<Result> DetectBarcodeAsync()
        {
            var width = (int)CaptureResolution.Width;
            var height = (int)CaptureResolution.Height;
            var previewBuffer = new byte[width * height];

            PhotoCaptureDevice.GetPreviewBufferY(previewBuffer);

            var barcodeReader = new BarcodeReader();
            barcodeReader.TryHarder = true;
            //barcodeReader.TryInverted = true;
            //barcodeReader.AutoRotate = true;

            var result = barcodeReader.Decode(previewBuffer, width, height, RGBLuminanceSource.BitmapFormat.Gray8);
            if (result != null)
            {
                if (!_notfocus)
                {
                    PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.LockedAutoFocusParameters, AutoFocusParameters.Focus);
                    _notfocus = true;
                }

                await Task.Delay(10);
                //System.Diagnostics.Debug.WriteLine(result.Text);
            }


            return result;
        }


        private async Task StartCapturingAsync()
        {
            try
            {
                CameraCaptureSequence sequence = PhotoCaptureDevice.CreateCaptureSequence(1);
                var memoryStream = new MemoryStream();
                sequence.Frames[0].CaptureStream = memoryStream.AsOutputStream();

                PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Off);
                PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.SceneMode, CameraSceneMode.Macro);

                await PhotoCaptureDevice.PrepareCaptureSequenceAsync(sequence);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }
        
        private async Task InitializePhotoCaptureDevice(Size size)
        {
            try
            {
                PhotoCaptureDevice = await PhotoCaptureDevice.OpenAsync(CameraSensorLocation.Back, size);


                CompositeTransform = new CompositeTransform();
                CompositeTransform.CenterX = .5;
                CompositeTransform.CenterY = .5;
                CompositeTransform.Rotation = PhotoCaptureDevice.SensorRotationInDegrees - 90;

                VideoBrush = new VideoBrush();
                VideoBrush.RelativeTransform = CompositeTransform;
                VideoBrush.Stretch = Stretch.Fill;
                // IMPORTANT: You need to add a namespace Microsoft.Devices to be able to 
                VideoBrush.SetSource(PhotoCaptureDevice);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }

        public VideoBrush VideoBrush
        {
            get { return _videoBrush; }
            set
            {
                if (Equals(value, _videoBrush))
                    return;
                _videoBrush = value;
                RaisePropertyChanged();
            }
        }
        private VideoBrush _videoBrush;

        public PhotoCaptureDevice PhotoCaptureDevice
        {
            get { return _photoCaptureDevice; }
            set
            {
                if (Equals(value, _photoCaptureDevice)) 
                    return;
                _photoCaptureDevice = value;
                RaisePropertyChanged();
            }
        }
        private PhotoCaptureDevice _photoCaptureDevice;

        public CompositeTransform CompositeTransform
        {
            get { return _compositeTransform; }
            set
            {
                if (Equals(value, _compositeTransform)) 
                    return;
                _compositeTransform = value;
                RaisePropertyChanged();
            }
        }
        private CompositeTransform _compositeTransform;

        public Size CaptureResolution
        {
            get { return _captureResolution; }
            set
            {
                if (value.Equals(_captureResolution)) 
                    return;
                _captureResolution = value;
                RaisePropertyChanged();
            }
        }
        private Size _captureResolution;

        public string Result
        {
            get { return _result; }
            private set
            {
                if (!ReferenceEquals(value, _result))
                {
                    _result = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _result;
    }
}
