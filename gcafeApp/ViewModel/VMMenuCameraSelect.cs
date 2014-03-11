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
using Microsoft.Phone.Shell;
using Windows.Phone.Media.Capture;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using gcafeApp.gcafeSvc;
using ZXing;

namespace gcafeApp.ViewModel
{
    public class VMMenuCameraSelect : VMBase
    {
        private readonly IgcafeSvcClient _svc;
        static bool isInit = false;
        bool _notfocus = false;
        bool _found = false;
        bool _break = false;
        DateTime _dt;
        Thread _thrFocus;
        Thread _thrDetect;

        public VMMenuCameraSelect(IgcafeSvcClient svc)
        {
            if (!IsInDesignMode)
            {
                _svc = svc;

                //_svc.GetMenuItemByNumberCompleted -= _svc_GetMenuItemByNumberCompleted;
                if (!isInit)
                {
                    _svc.GetMenuItemByNumberCompleted += _svc_GetMenuItemByNumberCompleted;
                    isInit = true;

                    System.Diagnostics.Debug.WriteLine("================================================= inittttttttttttttt");
                }
                //if (!isInit)
                //{
                //    _svc.GetMenuItemByNumberCompleted += new EventHandler<GetMenuItemByNumberCompletedEventArgs>((sender, e) =>
                //    {
                //        try
                //        {
                //            if (e.Result != null)
                //            {
                //                MenuItem menuItem = e.Result;
                //                this.Result = menuItem.Name;

                //                PhoneApplicationService.Current.State["SelectedMenuItem"] = menuItem;

                //                RaisePropertyChanged("SelectedMenuItem");

                //                //System.Diagnostics.Debug.WriteLine(this.Result);
                //            }
                //            else
                //            {
                //                this.Result = "此项目不存在或被锁";
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            string s = ex.Message;
                //        }

                //    });

                //    isInit = true;
                //}
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            _svc.GetMenuItemByNumberCompleted -= _svc_GetMenuItemByNumberCompleted;
        }

        void _svc_GetMenuItemByNumberCompleted(object sender, GetMenuItemByNumberCompletedEventArgs e)
        {
            if (ReferenceEquals(e.UserState, "VMMenuCameraSelect"))
            {
                try
                {
                    if (e.Result != null)
                    {
                        MenuItem menuItem = e.Result;
                        this.Result = menuItem.Name;

                        PhoneApplicationService.Current.State["SelectedMenuItem"] = menuItem;

                        RaisePropertyChanged("SelectedMenuItem");

                        //System.Diagnostics.Debug.WriteLine(this.Result);
                    }
                    else
                    {
                        this.Result = "此项目不存在或被锁";
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
        }

        public async void InitializeAndGo()
        {
            CaptureResolution = new Size(800, 480);
            await InitializePhotoCaptureDevice(CaptureResolution);
            await StartCapturingAsync();
            await PhotoCaptureDevice.FocusAsync();

            _break = false;

            //_found = false;

            System.Diagnostics.Debug.WriteLine(string.Format("init : ===== {0}", _break));
            int cnt = 0;
            while (!_break)
            {
                Result result = await GetBarcodeAsync();
                System.Diagnostics.Debug.WriteLine("scan: {0} times", cnt++);

                if (result != null)
                {
                    if (result.Text.Substring(0, 2) == "11" ||
                        result.Text.Substring(0, 2) == "22")
                    {
                        System.Diagnostics.Debug.WriteLine("cccccccccccccccccccccccccccallllllllllllllllllllllllllllllllll");
                        _svc.GetMenuItemByNumberAsync(Settings.AppSettings.DeviceID, result.Text, "VMMenuCameraSelect");
                        _break = true;
                    }
                }

                await Task.Delay(10);
            }

            PhotoCaptureDevice.Dispose();
            PhotoCaptureDevice = null;

            System.Diagnostics.Debug.WriteLine("======================================== break..");

            _break = false;

            //_thrFocus = new Thread(new ThreadStart(ThreadFocus));
            //_thrFocus.Start();
            //_thrDetect = new Thread(new ThreadStart(ThreadDetect));
            //_thrDetect.Start();

            //_thrDetect.Join();
        }

        public void Uninitialize()
        {
            _break = true;
        }

        async void ThreadFocus()
        {
            while (!_found)
            {
                if (PhotoCaptureDevice != null)
                {
                    await Task.Delay(100);
                    await PhotoCaptureDevice.FocusAsync();
                }
            }

            System.Diagnostics.Debug.WriteLine("=====================   Thread Focus exit, {0}", (DateTime.Now - _dt).TotalMilliseconds);

            PhotoCaptureDevice.Dispose();
            PhotoCaptureDevice = null;

            System.Diagnostics.Debug.WriteLine("=====================   Thread Focus exit, {0}", (DateTime.Now - _dt).TotalMilliseconds);
        }

        async void ThreadDetect()
        {
            int cnt = 0;
            while (!_found)
            {
                Result result = await DetectBarcodeAsync();

                System.Diagnostics.Debug.WriteLine(string.Format("========= {0}", cnt++));

                if (result != null)
                {
                    if (result.Text.Substring(0, 2) == "11" ||
                        result.Text.Substring(0, 2) == "22")
                    {
                        _dt = DateTime.Now;
                        System.Diagnostics.Debug.WriteLine("dddddddddddddddddddddtect 0000000000");
                        //_svc.GetMenuItemByNumberAsync(Settings.AppSettings.DeviceID, result.Text);
                        _found = true;
                    }
                }
            }

            _thrFocus.Join();

            System.Diagnostics.Debug.WriteLine("====================== Thread Detect exit");
        }

        private async Task<Result> GetBarcodeAsync()
        {
            //if (!_notfocus)
            //await PhotoCaptureDevice.FocusAsync();
            //PhotoCaptureDevice.FocusAsync();
            return await DetectBarcodeAsync();
        }

        private async Task<Result> DetectBarcodeAsync()
        {
            var width = (int)CaptureResolution.Width;
            var height = (int)CaptureResolution.Height;
            var previewBuffer = new byte[width * height];

            PhotoCaptureDevice.GetPreviewBufferY(previewBuffer);

            WriteableBitmap bmp;

            var barcodeReader = new BarcodeReader();
            //barcodeReader.TryHarder = true;
            //barcodeReader.TryInverted = true;
            //barcodeReader.AutoRotate = true;

            var result = barcodeReader.Decode(previewBuffer, width, height, RGBLuminanceSource.BitmapFormat.Gray8);
            if (result != null)
            {
                if (result.Text.Substring(0, 2) == "11" ||
                    result.Text.Substring(0, 2) == "22")
                {
                    if (!_notfocus)
                    {
                        PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.LockedAutoFocusParameters, AutoFocusParameters.Focus);
                        _notfocus = true;
                    }
                }
                //await Task.Delay(10);
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
                CompositeTransform.Rotation = 0;
                CompositeTransform.ScaleX = 1.5;
                CompositeTransform.ScaleY = 1.5;
                //CompositeTransform.Rotation = PhotoCaptureDevice.SensorRotationInDegrees - 90;

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
