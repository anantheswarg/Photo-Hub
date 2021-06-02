using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using PhotoHub;

namespace PhotoHub
{
    public static class SettingsHelper
    {
        public static string AuthenticationToken
        {
            get
            {
                string setting = string.Empty;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("authToken", out setting);
                return setting;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["authToken"] = value;
            }
        }

        public static string ImageFilename
        {
            get
            {
                if (PhoneApplicationService.Current.State.ContainsKey("imageFilename"))
                {
                    return PhoneApplicationService.Current.State["imageFilename"] as string;
                }

                string filename = GenerateRandomImageName();

                ImageFilename = filename;

                return filename;
            }
            set
            {
                PhoneApplicationService.Current.State["imageFilename"] = value;
            }
        }

        private static string GenerateRandomImageName()
        {
            return (new Random().Next(1, 10000)) + ".jpg";
        }

        public static UploadDataViewModel UploadDataViewModel
        {
            get
            {
                object data;

                if (PhoneApplicationService.Current.State.TryGetValue("Data", out data))
                    return data as UploadDataViewModel;
                else
                    return new UploadDataViewModel();
            }
            set
            {
                PhoneApplicationService.Current.State["Data"] = value;
            }
        }
    }
}
