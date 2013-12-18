using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using gcafeApp.gcafeSvc;
using Microsoft.Phone.Info;

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

        public static string DeviceID
        {
            get
            {
                object id;
                if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out id))
                    return Convert.ToBase64String((byte[])id);
                else
                    throw new Exception("取UniqueID出错");
            }
        }
    }
}
