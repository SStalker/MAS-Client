using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Net.Sockets;



namespace MAS_Client
{
    class Command
    {
        static NetworkStream stream;

        public static void setStream(ref NetworkStream s)
        {
            stream = s;
        }

        const int MAX_PATH = 260;
        const int SPI_GETDESKWALLPAPER = 0x73;
        const int SPI_SETDESKWALLPAPER = 0x14;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        const int APPCOMMAND_VOLUME_UP = 0xA0000;
        const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        const int WM_APPCOMMAND = 0x319;
        const int APPCOMMAND_MEDIA_PLAY_PAUSE = 0xE0000;

        const int WM_SETTEXT = 0x000c;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int hwndParent, int hwndEnfant, int lpClasse, string lpTitre);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // Not ready to gooooo
        public static void GetIPAdress()
        {
            IPHostEntry host;
            //List<string> ips;

            //string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    Console.WriteLine(ip.ToString());
                }
            }
        }

        public static void NetworkScan(String sourceIP) 
        {
            int lastDotPos = sourceIP.LastIndexOf('.');
            sourceIP = sourceIP.Remove(lastDotPos);

            Console.WriteLine(sourceIP);
            Ping Sender = new Ping();

            for (int i = 1; i < 255; i++)
            {
                String ip = sourceIP + "." + i.ToString();
                PingReply Result = Sender.Send(ip,500);
                if (Result.Status == IPStatus.Success)
                    Console.WriteLine("Erfolg:{0}",ip);
            }

            Console.WriteLine("Scan closed");
         }


        public static void SendMessageToActiveWindow(string message) 
        {
            SendKeys.SendWait(message);

            //SendKeys.SendWait("%{Alt Down}");
            //SendKeys.SendWait("%{TAB}");
            //SendKeys.SendWait("%{Alt Up}");
        }

       
        public static void GetNetworkComputers()
        {
            IPGlobalProperties network = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = network.GetActiveTcpConnections();

            foreach (TcpConnectionInformation t in connections)
            {
                Console.Write("Local endpoint: {0} ", t.LocalEndPoint.Address);
                Console.Write("Remote endpoint: {0} ", t.RemoteEndPoint.Address);
                Console.WriteLine("{0}", t.State);
            }
        }

        public static void SetActiveWindow(int handle) 
        {
            IntPtr hWnd = new IntPtr(handle); 
            SetForegroundWindow(hWnd);
            SetActiveWindow(hWnd);

            
        }

        public static int GetHandleFromWindow(string title)
        {
            int hwnd = FindWindowEx(0, 0, 0, title);//where title is the windowtitle

            //verification of the window
            if (hwnd == 0)
            {
                return -1;
                //throw new Exception("Window not found");
            }

            return hwnd;
        }

        public static void GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                Byte[] prog = System.Text.Encoding.Unicode.GetBytes(Buff.ToString());
                stream.Write(prog, 0, prog.Length);
            }
        }



        public static void GetOSPlatform()
        {
            OperatingSystem os = Environment.OSVersion;

            PlatformID platform = os.Platform;
            Byte[] plat = ToNetStr(platform.ToString());
            stream.Write(plat,0,plat.Length);
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


        public static void StartProgram(string exe)
        {
            Process prog = new Process();

            prog.StartInfo.FileName = exe;
            prog.StartInfo.Arguments = "";

            prog.Start();
        }

        public static void GetOpenPrograms()
        {
            string output = "";
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        output += "\r\n";
                        output +="\r\n Window Title:" + p.MainWindowTitle.ToString();
                        output +="\r\n Process Name:" + p.ProcessName.ToString();
                        output +="\r\n Window Handle:" + p.MainWindowHandle.ToString();
                        output +="\r\n Memory Allocation:" + p.PrivateMemorySize64.ToString();
                        output += "\r\n Process ID:" + p.Id.ToString();
                    }
                }
                catch { }
            }

            output += "\0";
            Byte[] data = System.Text.Encoding.Unicode.GetBytes(output);
            stream.Write(data, 0, data.Length);
        }


        public static void KillProcess(string name)
        {
            Byte[] killed = System.Text.Encoding.Unicode.GetBytes("The processes were killed");
            Byte[] notKilled = System.Text.Encoding.Unicode.GetBytes("There is no process named {" + name + "}");
            

            Process[] procs = Process.GetProcessesByName(name);

            if (procs.Length == 0)
            {
                stream.Write(notKilled,0,notKilled.Length);
            }
            else
            {
                foreach( Process p in procs)   
                p.Kill();

                stream.Write(killed, 0, killed.Length);
            }
        }

        // Turns an string to Byte[]
        private static Byte[] ToNetStr(string text)
        {
            return System.Text.Encoding.Unicode.GetBytes(text);
        }
    }
}
