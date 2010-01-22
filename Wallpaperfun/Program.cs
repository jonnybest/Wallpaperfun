using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading;

namespace Wallpaperfun
{
    class Program
    {
        // wir schummeln hier ein bisschen, damit der externe aufruf schneller läuft ;)
        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // der hier darf ruhig langsam laufen
        [DllImport("User32.dll")]
        public static extern int MessageBox(int h, string m, string c, int type);

        static void Main(string[] args)
        {
            string userImage;
            bool replaceImage = true;

            if (args.Length > 0)
            {
                userImage = Path.GetFullPath(args[0]);
            }
            else
            {
                userImage = "";
            }
            string myImage = SelectRandomPicture( System.IO.Directory.GetCurrentDirectory() );

            if (myImage == null)
            {
                replaceImage = false;
            }
            else if (myImage == GetCurrentWallpaper())
            {
                myImage = SelectRandomPicture( System.IO.Directory.GetCurrentDirectory() );
            }

            if (userImage.Length > 0) myImage = userImage;

            if (replaceImage)
            {
                SetWallpaper(myImage, 0, 2);
            }
        }

        static private string GetCurrentWallpaper()
        {
            // The current wallpaper path is stored in the registry at HKEY_CURRENT_USER\\Control Panel\\Desktop\\WallPaper
            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string WallpaperPath = rkWallPaper.GetValue("WallPaper").ToString();
            rkWallPaper.Close();
            // Return the current wallpaper path
            return WallpaperPath;
        }

        static private void SetWallpaper(string WallpaperLocation, int WallpaperStyle, int TileWallpaper)
        {
            // Sets the actual wallpaper            
            SystemParametersInfo(20, 0, WallpaperLocation, 0x01 | 0x02);
        }

        static private string SelectRandomPicture(string PathLocation)
        {
            Random myGenerator = new Random();
            string[] myImages = Directory.GetFiles(PathLocation, "*.jpg");
            if (myImages.Length == 0)
            {
                MessageBox(0, "An error occured: I could not find any images in this folder. \nPlease make sure, all images are in JPG format.\nMy current folder is "+PathLocation, "Error in Wallpaperfun", 0);
                return null;
            }
            else
            {
                return myImages[myGenerator.Next(myImages.Length)];
            }
        }

        static private void AutomatischDurchschalten(TimeSpan TotalDuration, TimeSpan SleepTime)
        {
            int numberoftimes = 0;
            TimeSpan tempTime = TotalDuration.Duration();
            while (tempTime > TimeSpan.Zero)
            {
                tempTime = tempTime.Subtract(SleepTime);
                numberoftimes++;
            }

            AutomatischDurchschalten(SleepTime, numberoftimes);
        }

        static private void AutomatischDurchschalten(TimeSpan SleepTime, int nMal)
        {
            for (int i = 0; i < nMal; i++)
			{
                bool replaceImage = true;

                string myImage = SelectRandomPicture(System.IO.Directory.GetCurrentDirectory());

                if (myImage == null)
                {
                    replaceImage = false;
                }
                else if (myImage == GetCurrentWallpaper())
                {
                    myImage = SelectRandomPicture(System.IO.Directory.GetCurrentDirectory());
                }

                if (replaceImage)
                {
                    SetWallpaper(myImage, 0, 2);
                }

                Thread.Sleep(SleepTime);
            }
        }
    }
}
