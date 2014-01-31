/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:gcafeApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.ServiceModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<IgcafeSvcClient>(() =>
                {
                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.MaxBufferSize = 2147483647;
                    binding.MaxReceivedMessageSize = 2147483647;
                    //EndpointAddress address = new EndpointAddress("http://192.168.15.210/gcafeSvc.svc");
                    //EndpointAddress address = new EndpointAddress("http://192.168.15.100:8733/Design_Time_Addresses/gcafeSvcFoxpro/gcafeSvc/");
                    EndpointAddress address = new EndpointAddress(string.Format("http://{0}/Design_Time_Addresses/gcafeFoxproSvc/gcafeSvc/", gcafeApp.Settings.AppSettings.ServiceURL));

                    return new IgcafeSvcClient(binding, address);
                });

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<TablesViewModel>();
            SimpleIoc.Default.Register<MenuSelectViewModel>();
            SimpleIoc.Default.Register<VMLogin>();
            SimpleIoc.Default.Register<VMMenuCatalog>();
            SimpleIoc.Default.Register<VMSubMenuCatalog>();
            SimpleIoc.Default.Register<VMMenuItem>();
            SimpleIoc.Default.Register<VMMenuCameraSelect>();
            SimpleIoc.Default.Register<VMSelectMethod>();
            SimpleIoc.Default.Register<VMBillDetail>();
            SimpleIoc.Default.Register<VMMenuOption>();
            SimpleIoc.Default.Register<VMChangeTable>();
            SimpleIoc.Default.Register<VMPrintLiuTaiDan>();
            SimpleIoc.Default.Register<VMBilling>();
        }

        public VMBilling VMBilling
        {
            get { return ServiceLocator.Current.GetInstance<VMBilling>(); }
        }

        public VMPrintLiuTaiDan VMPrintLiuTaiDan
        {
            get { return ServiceLocator.Current.GetInstance<VMPrintLiuTaiDan>(); }
        }

        public VMChangeTable VMChangeTable
        {
            get { return ServiceLocator.Current.GetInstance<VMChangeTable>(); }
        }

        public VMMenuOption VMMenuOption
        {
            get { return ServiceLocator.Current.GetInstance<VMMenuOption>(); }
        }

        public VMBillDetail VMBillDetail
        {
            get { return ServiceLocator.Current.GetInstance<VMBillDetail>(); }
        }

        public VMSelectMethod VMSelectMethod
        {
            get { return ServiceLocator.Current.GetInstance<VMSelectMethod>(); }
        }

        public VMMenuCameraSelect VMMenuCameraSelect
        {
            get { return ServiceLocator.Current.GetInstance<VMMenuCameraSelect>(); }
        }

        public VMMenuItem VMMenuItem
        {
            get { return ServiceLocator.Current.GetInstance<VMMenuItem>(); }
        }

        public VMSubMenuCatalog VMSubMenuCatalog
        {
            get { return ServiceLocator.Current.GetInstance<VMSubMenuCatalog>(); }
        }

        public VMMenuCatalog VMMenuCatalog
        {
            get { return ServiceLocator.Current.GetInstance<VMMenuCatalog>(); }
        }

        public MenuSelectViewModel VMMenuSelect
        {
            get { return ServiceLocator.Current.GetInstance<MenuSelectViewModel>(); }
        }

        public TablesViewModel TablesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<TablesViewModel>(); }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public VMLogin VMLogin
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMLogin>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}