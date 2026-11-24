using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPSocketLesson
{
    internal class Program
    {
        /*static List<Socket> clients = new List<Socket>();              // все клиенты
        static List<string> messages = new List<string>();             // общие сообщения
        static object locker = new object();*/
        static async Task Main(string[] args)
        {
            using Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Random random = new Random();
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
            //serverSocket.Listen(10);

            byte[] buffer = new byte[1024];
            
            EndPoint clientEP = new IPEndPoint(IPAddress.Loopback, 5000);

            Console.WriteLine("Server UDP is started");


            //Using without Async
            /*
            int recieved = serverSocket.ReceiveFrom(buffer,ref clientEP);
            string msg = Encoding.UTF8.GetString(buffer,0 , recieved);
            Console.WriteLine($"From Client {clientEP}: {msg}");
            Thread.Sleep(1000);
            byte[] reply = Encoding.UTF8.GetBytes("Echo: " + msg);
            serverSocket.SendTo(reply, clientEP);
            */
            int randomNumber = random.Next(0, 101);
                

                while (true)
                {
                    // ждём сообщение от клиента
                    SocketReceiveFromResult recieved = await serverSocket.ReceiveFromAsync(
                    new ArraySegment<byte>(buffer), SocketFlags.None, clientEP);
                    var remoteEp = recieved.RemoteEndPoint;
                    string msg = Encoding.UTF8.GetString(buffer, 0, recieved.ReceivedBytes);

                    if (int.TryParse(msg, out int number))
                    {
                        if (number == randomNumber)
                        {
                            string reply = $"Congratulations! Player {clientEP} guessed the number!";
                            Console.WriteLine(reply);

                            byte[] replyBytes = Encoding.UTF8.GetBytes(reply);
                            await serverSocket.SendToAsync(replyBytes, SocketFlags.None, clientEP);

                            break;
                        }
                        else if (number > randomNumber)
                        {
                            string reply = "Too high!";
                            Console.WriteLine($"[{clientEP}] guessed {number} too high");
                            await serverSocket.SendToAsync(Encoding.UTF8.GetBytes(reply), SocketFlags.None, clientEP);
                        }
                        else
                        {
                            string reply = "Too low!";
                            Console.WriteLine($"[{clientEP}] guessed {number} too low");
                            await serverSocket.SendToAsync(Encoding.UTF8.GetBytes(reply), SocketFlags.None, clientEP);
                        }
                    }
                    else
                    {
                        string reply = "Invalid input, enter a number!";
                        await serverSocket.SendToAsync(Encoding.UTF8.GetBytes(reply), SocketFlags.None, clientEP);
                    }
                }

            


            
        }
    }
}