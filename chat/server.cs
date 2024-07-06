using System;
using System.Collections.Generic; // Используется для работы со списками.
using System.Net; // Включает классы для работы с сетевыми адресами.
using System.Net.Sockets; // Включает классы для работы с сокетами.
using System.Text; // Используется для кодирования строк.
using System.Threading; // Используется для работы с потоками.

namespace chat
{
    class Server
    {
        private static TcpListener listener;
        private static List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {

            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Server started...");

            Thread clientAcceptThread = new Thread(new ThreadStart(AcceptClients));
            clientAcceptThread.Start();

            Console.WriteLine("Нажмите любую клавишу для остановки сервера.");
            Console.ReadLine();

            listener.Stop();

        }

        private static void AcceptClients()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine("Client connected.");

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        private static void HandleClient(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int byteCount;
            string clientMessage;

            try
            {
                while ((byteCount = stream.Read(buffer, 0 , buffer.Length)) > 0)
                {
                    clientMessage = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("Sended message: " + clientMessage);

                    BroadcastMessage(clientMessage);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client.Close();
                clients.Remove(client);
                Console.WriteLine("Client disconnected.");
            }

        }

        private static void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
