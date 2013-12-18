using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using gcafeApp.gcafeSvc;

namespace gcafeApp.Settings
{
    public static class AppSettings
    {
        public static Staff LoginStaff
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("LoginStaff"))
                    return IsolatedStorageSettings.ApplicationSettings["LoginStaff"] as Staff;
                else
                    return (Staff)null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["LoginStaff"] = value;
            }
        }

    }
}
