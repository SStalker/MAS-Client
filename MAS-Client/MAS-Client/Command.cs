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



namespace MAS_Client
{
    class Command
    {
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
        /*public static List<string> getIPAdress()
        {
            IPHostEntry host;
            List<string> ips;

            //string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    ips.Add(ip.ToString());
                }
            }
            return ips;
        }*/

        public static void ShowNetworkInterfaces()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);
            if (nics == null || nics.Length < 1)
            {
                Console.WriteLine("  No network interfaces found.");
                return;
            }

            Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
            foreach (NetworkInterface adapter in nics)
            {


                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}",
                    adapter.OperationalStatus);
                string versions = "";

                UnicastIPAddressInformationCollection uniCol = properties.UnicastAddresses;

                foreach(UnicastIPAddressInformation uniAdress in uniCol)
                {
                    Console.WriteLine("  IP-Adress ............................... : {0}", uniAdress.Address);
                    Console.WriteLine("  Subnet Mask: ............................ : {0}", uniAdress.IPv4Mask);
                    
                }
                // Create a display string for the supported IP versions. 
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);
                //ShowIPAddresses(properties);
               

                // The following information is not useful for loopback adapters. 
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                Console.WriteLine("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix);

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            //ShowIPAddresses(label, winsServers);
                        }
                    }
                }

                Console.WriteLine("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled);
                Console.WriteLine("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled);
                Console.WriteLine("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast);
                //ShowInterfaceStatistics(adapter);

                Console.WriteLine();
            }
        }

        public static void sendMessageToActiveWindow(string message) 
        {
            SendKeys.SendWait(message);

            //SendKeys.SendWait("%{Alt Down}");
            //SendKeys.SendWait("%{TAB}");
            //SendKeys.SendWait("%{Alt Up}");
        }

       
        public static void getNetworkComputers()
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

        public static void setActiveWindow(int handle) 
        {
            IntPtr hWnd = new IntPtr(handle); 
            SetForegroundWindow(hWnd);
            SetActiveWindow(hWnd);

            
        }

        public static int getHandleFromWindow(string title)
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

        public static string getActiveWindowTitle()
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

        public static string getOSPlatform()
        {
            OperatingSystem os = Environment.OSVersion;

            PlatformID platform = os.Platform;

            return platform.ToString();
        }

        public static void setWallpaper(string pathToFile)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, pathToFile, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static string getWallpaper()
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
