using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace MAS_Client
{
    class Program
    {
        static NetworkStream stream;
        static Int32 port = 13000;
        static TcpClient client;
        static String serverIP = "192.168.2.101";
        static Byte[] data;


        static void Main(string[] args)
        {
            //SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);

            Console.WriteLine(Command.getOSPlatform());
            Command.SetWallpaper(@"C:\Users\SStalker\Pictures\wallbase cc\wallpaper-1353834.jpg");

            Command.startProgram("notepad.exe",null);

            Console.WriteLine(Command.GetWallpaper());

            Console.WriteLine(Command.GetActiveWindowTitle());

            Command.getOpenPrograms();
            Connect(serverIP);
            
            if (client != null)
            {
                while(client.Connected)
                {                    
                    if (stream.DataAvailable) 
                    {
                        // Buffer to store the response bytes.
                        data = new Byte[256];

                        // String to store the response ASCII representation.
                        String responseData = String.Empty;

                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        Console.WriteLine("Received: {0}", responseData);
                    }                    
                }
            }
        }

        static void Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                client = new TcpClient(server, port);

                // Get a client stream for reading and writing
                stream = client.GetStream();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}


