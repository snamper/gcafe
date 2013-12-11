using System;
using GalaSoft.MvvmLight;
using Microsoft.Phone.Info;

namespace gcafeApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        /// <summary>
        /// Gets the unique hash for the device.
        /// </summary>
        public string DeviceUniqueID
        {
            get
            {
                object id;
                if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out id))
                    return Convert.ToBase64String((byte[])id);
                else
                    throw new Exception("È¡UniqueID³ö´í");
            }
        }

    }
}