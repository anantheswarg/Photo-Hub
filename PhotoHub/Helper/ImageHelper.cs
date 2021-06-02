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
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace PhotoHub
{
    public static class ImageHelper
    {
        public const string IncomingDirectory = "Incoming";
        public const string ImageDirectory = "Images";

        public static Stream LoadImage(string directory, string fileName)
        {
            string path = System.IO.Path.Combine(directory, fileName);
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(directory)) return null;
                if (!isoStore.FileExists(path)) return null;

                IsolatedStorageFileStream isolatedStorageFileStream = isoStore.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                return isolatedStorageFileStream;
            }
        }

        public static void SaveImage(Stream imageStream, string directory, string fileName)
        {
            if (imageStream != null)
            {
                string path = System.IO.Path.Combine(directory, fileName);

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists(directory)) isoStore.CreateDirectory(directory);

                    using (var writeStream = isoStore.CreateFile(path))
                    {
                        byte[] buffer = new byte[32768];
                        while (true)
                        {
                            int read = imageStream.Read(buffer, 0, buffer.Length);
                            if (read <= 0)
                                return;
                            writeStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }

        public static bool ImageExists(string directory, string fileName)
        {
            string path = System.IO.Path.Combine(directory, fileName);
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.FileExists(path);
            }
        }

        public static void DeleteImage(string directory, string fileName)
        {
            string path = System.IO.Path.Combine(directory, fileName);
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(directory)) return;
                isoStore.DeleteFile(path);
            }
        }
    }
}
