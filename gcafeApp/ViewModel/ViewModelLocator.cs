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
                    return new IgcafeSvcClient();
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