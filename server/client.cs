using System;
using System.Net.Sockets; // Включает классы для работы с сокетами.
using System.Text; // Используется для кодирования строк.
using System.Threading; // Используется для работы с потоками.

namespace chat
{
    class Client
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Enter IP-adress of server.");
            string ipAdress = Console.ReadLine();

            try
            {
                TcpClient client = new TcpClient(ipAdress, 8888);
                NetworkStream stream = client.GetStream();

                Thread acceptMessages = new Thread(() => AcceptMessages(client));
                acceptMessages.Start();

                Console.WriteLine("Connected to server.");

                string message;
                byte[] buffer;

                while (true)
                {
                    message = Console.ReadLine();

                    if (message.ToLower() == "exit")
                    {
                        break;
                    }

                    buffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }

                client.Close();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AcceptMessages(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            int byteCount;
            byte[] buffer = new byte[1024];

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length))>0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("Message from server :" + message);
                }
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }
        }
    }
}
