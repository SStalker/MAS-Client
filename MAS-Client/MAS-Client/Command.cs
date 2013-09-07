using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MAS_Client
{
    class Command
    {
        const int MAX_PATH = 260;
        const int SPI_GETDESKWALLPAPER = 0x73;
        const int SPI_SETDESKWALLPAPER = 0x14;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public static void ausgabe(string text)
        {
            Console.WriteLine(text);
        }

        public static string getOSPlatform()
        {
            OperatingSystem os = Environment.OSVersion;

            PlatformID platform = os.Platform;

            return platform.ToString();
        }

        public static void SetWallpaper(string pathToFile)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, pathToFile, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static string GetWallpaper()
        {
            string wallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
            return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        }


        public static void startProgram(string exe, Array args)
        {
            Process prog = new Process();

            prog.StartInfo.FileName = exe;
            prog.StartInfo.Arguments = "";

            prog.Start();
        }

        public static void getOpenPrograms()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        Console.WriteLine("\r\n");
                        Console.WriteLine("\r\n Window Title:" + p.MainWindowTitle.ToString());
                        Console.WriteLine("\r\n Process Name:" + p.ProcessName.ToString());
                        Console.WriteLine("\r\n Window Handle:" + p.MainWindowHandle.ToString());
                        Console.WriteLine("\r\n Memory Allocation:" + p.PrivateMemorySize64.ToString());
                    }
                }
                catch { }
            }
        }
    }
}
