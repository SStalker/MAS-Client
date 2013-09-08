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

            /*Console.WriteLine(Command.getOSPlatform());
            Command.setWallpaper(@"C:\Users\SStalker\Pictures\wallbase cc\wallpaper-1353834.jpg");

            for (int i = 0; i < 100; i++)
            {
                //Command.startProgram("notepad.exe", null);
            }
            Console.WriteLine(Command.getWallpaper());

            Console.WriteLine(Command.getActiveWindowTitle());

            Command.getOpenPrograms();

            Command.ShowNetworkInterfaces();
            //Console.WriteLine(Command.getHandleFromWindow("Rechner"));

            int specialHandle = Command.getHandleFromWindow("Editor");
            System.Threading.Thread.Sleep(5000);
            Command.setActiveWindow(specialHandle);
            Command.sendMessageToActiveWindow("Hallo ");
            System.Threading.Thread.Sleep(2000);
            Command.sendMessageToActiveWindow("Chris!!! :D");
            //Command.sendMessageToActiveWindow("{DELETE}");
            

            Command.getNetworkComputers();
            */


            Connect(serverIP);
            
            if (client != null)
            {
                while(client.Connected)
                {                    
                    if (stream.DataAvailable) 
                    {
                        // Buffer to store the response bytes.
                        data = new Byte[256];

                        // String to store the response Unicode representation.
                        String responseData = String.Empty;

                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
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


